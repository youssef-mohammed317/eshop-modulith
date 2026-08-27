namespace Ordering.Ordering.Events;

public record OrderStatusChangedEvent(Guid Id, OrderStatus OrderStatus) : IDomainEvent;