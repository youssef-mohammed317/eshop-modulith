using Shared.Messaging.Events;

namespace BuildingBlocks.Messaging.Events;

/// <summary>
/// Event published when a user checks out their basket.
/// Subscribed by the Ordering microservice to create a new order.
/// </summary>
public record BasketCheckoutIntegrationEvent : IntegrationEvent
{
    // User & Basket Info
    public string UserName { get; set; } = default!;
    public Guid CustomerId { get; set; } = default!;
    public decimal TotalPrice { get; set; } = default!;

    // Shipping and Billing Address Info
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string EmailAddress { get; set; } = default!;
    public string AddressLine { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string State { get; set; } = default!;
    public string ZipCode { get; set; } = default!;

    // Payment Info
    public string CardName { get; set; } = default!;
    public string CardNumber { get; set; } = default!;
    public string Expiration { get; set; } = default!;
    public string Cvv { get; set; } = default!;
    public int PaymentMethod { get; set; } = default!;
}