using Microsoft.Extensions.Logging;


namespace Ordering.Ordering.EventHandlers.Domain;

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