namespace Basket.Basket.Dtos; // Adjust namespace to match your structure

public record BasketCheckoutDto(
    // User & Basket Info
    string UserName,
    Guid CustomerId,
    decimal TotalPrice,

    // Shipping and Billing Address Info
    string FirstName,
    string LastName,
    string EmailAddress,
    string AddressLine,
    string Country,
    string State,
    string ZipCode,

    // Payment Info
    string CardName,
    string CardNumber,
    string Expiration,
    string Cvv,
    int PaymentMethod
);