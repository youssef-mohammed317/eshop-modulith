using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;


namespace Ordering.Ordering.EventHandlers.Domain;

public class OrderCreatedEventHandler
    (IBus publishEndpoint, IFeatureManager featureManager, ILogger<OrderCreatedEventHandler> logger)
    : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        // 1. Log the event
        logger.LogInformation("Domain Event Handled: {DomainEvent} for Order {OrderId}",
            domainEvent.GetType().Name,
            domainEvent.Order.Id);

        if (await featureManager.IsEnabledAsync("OrderFullfilment"))
        {
            // 2. Convert Domain Entity to DTO
            var orderDto = domainEvent.Order.ToOrderDto();

            // 3. Publish an Integration Event to the Message Broker (RabbitMQ)
            await publishEndpoint.Publish(orderDto, cancellationToken);
        }
    }
}
