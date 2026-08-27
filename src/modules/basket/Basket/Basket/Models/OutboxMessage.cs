using System;

namespace Shared.Messaging.Outbox;

public class OutboxMessage : Entity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The fully qualified name or type of the event (e.g., "OrderCreatedIntegrationEvent").
    /// </summary>
    public string Type { get; set; } = default!;

    /// <summary>
    /// The JSON serialized payload of the event.
    /// </summary>
    public string Content { get; set; } = default!;

    /// <summary>
    /// The exact UTC timestamp when the domain event occurred.
    /// </summary>
    public DateTime OccurredOn { get; set; }

    /// <summary>
    /// The UTC timestamp when the message was successfully published to the message broker.
    /// Null if it hasn't been processed yet.
    /// </summary>
    public DateTime? ProcessedOn { get; set; }
}