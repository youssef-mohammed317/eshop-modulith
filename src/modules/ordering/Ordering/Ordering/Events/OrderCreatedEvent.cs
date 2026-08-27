namespace Ordering.Ordering.Events;

public record OrderCreatedEvent(Order Order) : IDomainEvent;
