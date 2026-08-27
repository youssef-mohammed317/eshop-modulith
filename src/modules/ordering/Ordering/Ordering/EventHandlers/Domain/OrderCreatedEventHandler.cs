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

public class OrderStatusChangedEventHandler(ILogger<OrderStatusChangedEventHandler> logger)
    : INotificationHandler<OrderStatusChangedEvent>
{
    public Task Handle(OrderStatusChangedEvent domainEvent, CancellationToken cancellationToken)
    {
        // 1. Log the event with the new status
        logger.LogInformation("Domain Event Handled: {DomainEvent} for Order {OrderId}. New Status: {Status}",
            domainEvent.GetType().Name,
            domainEvent.Id,
            domainEvent.OrderStatus);

        // 2. Add side-effect logic:
        // - Send an SMS/Email to the user: "Your order is now on the way!"
        // - Inform the accounting microservice if status is 'Paid'

        return Task.CompletedTask;
    }
}