namespace Ordering.Ordering.Events;

public record OrderUpdatedEvent(Order Order) : IDomainEvent;
