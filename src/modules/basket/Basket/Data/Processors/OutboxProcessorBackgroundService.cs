using System.Text.Json;
using Basket.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.BackgroundJobs;

public class OutboxProcessorBackgroundService(
    IServiceScopeFactory serviceScopeFactory, IBus bus,
    ILogger<OutboxProcessorBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Executes the loop every 5 seconds
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessOutboxMessagesAsync(stoppingToken);


        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        // 1. Create a fresh scope for scoped dependencies
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();

        // 2. Fetch a batch of unprocessed messages (oldest first)
        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(20) // Process in batches to prevent memory spikes
            .ToListAsync(stoppingToken);

        if (messages.Count == 0)
        {
            return; // Nothing to process
        }

        // 3. Process and publish each message
        foreach (var message in messages)
        {
            try
            {
                // Resolve the exact C# Type from the stored string
                var eventType = Type.GetType(message.Type);

                if (eventType is null)
                {
                    logger.LogWarning("Could not resolve type: {Type} for OutboxMessage {Id}", message.Type, message.Id);
                    continue;
                }

                // Deserialize the JSON payload back into the Integration Event
                var integrationEvent = JsonSerializer.Deserialize(message.Content, eventType);

                if (integrationEvent is null)
                {
                    logger.LogWarning("Could not deserialize content for OutboxMessage {Id}", message.Id);
                    continue;
                }

                // Publish to RabbitMQ using the dynamic type overload
                await bus.Publish(integrationEvent, eventType, stoppingToken);

                // Mark as successfully processed
                message.ProcessedOn = DateTime.UtcNow;

                logger.LogInformation("Successfully published OutboxMessage {Id} of type {Type}", message.Id, eventType.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process OutboxMessage {Id}", message.Id);
                // Do not throw here. We want to log the error and allow the loop to continue
                // or retry on the next tick depending on your error handling strategy.
            }
        }

        // 4. Commit the 'ProcessedOn' updates to the database
        await dbContext.SaveChangesAsync(stoppingToken);
    }
}