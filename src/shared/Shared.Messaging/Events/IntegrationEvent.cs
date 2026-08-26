namespace Shared.Messaging.Events;

/// <summary>
/// Base record for all integration events across microservices.
/// </summary>
public abstract record IntegrationEvent
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the type of the event, useful for deserialization and routing.
    /// </summary>
    public string EventType => GetType().Name;
}
