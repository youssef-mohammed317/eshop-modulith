using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace Basket.Basket.EventConsumers;

public class ProductPriceChangedIntegrationEventConsumer(
    ISender sender,
    ILogger<ProductPriceChangedIntegrationEventConsumer> logger)
    : IConsumer<ProductPriceChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
    {
        logger.LogInformation("Processing price update for ProductId: {ProductId}", context.Message.ProductId);

        var command = new UpdateItemPriceInBasketCommand(
            context.Message.ProductId,
            context.Message.Price);

        var result = await sender.Send(command, context.CancellationToken);

        if (!result.IsSuccess)
        {
            logger.LogError("Failed to update price for ProductId: {ProductId} to new price: {Price}",
                context.Message.ProductId,
                context.Message.Price);

            // Throwing an exception ensures MassTransit marks the message as failed,
            // triggering retries or moving it to a _error queue.
            //throw new Exception($"Failed to update price in baskets for product {context.Message.ProductId}");
        }

        logger.LogInformation("Successfully updated price for ProductId: {ProductId}", context.Message.ProductId);
    }
}