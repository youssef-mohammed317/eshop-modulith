namespace Ordering.Ordering.ValueObjects;

public record Payment
{
    public string CardName { get; } = default!;
    public string CardNumber { get; } = default!;
    public string Expiration { get; } = default!;
    public string Cvv { get; } = default!;
    public int PaymentMethod { get; }

    // Protected parameterless constructor required by Entity Framework Core
    protected Payment() { }

    // Private constructor to force the use of the factory method
    private Payment(string cardName, string cardNumber, string expiration, string cvv, int paymentMethod)
    {
        CardName = cardName;
        CardNumber = cardNumber;
        Expiration = expiration;
        Cvv = cvv;
        PaymentMethod = paymentMethod;
    }

    /// <summary>
    /// Factory method to create a new Payment instance with validation.
    /// </summary>
    public static Payment Of(string cardName, string cardNumber, string expiration, string cvv, int paymentMethod)
    {
        // Validation rules using ArgumentException
        if (string.IsNullOrWhiteSpace(cardName))
            throw new ArgumentException("Card name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException("Card number cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(expiration))
            throw new ArgumentException("Expiration date cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(cvv))
            throw new ArgumentException("CVV cannot be null or empty.");

        // Additional business validations can be added here
        if (cvv.Length is < 3 or > 4)
            throw new ArgumentException("CVV must be 3 or 4 characters long.");

        // Assuming PaymentMethod shouldn't be negative or 0 (if 1 is your starting enum value)
        if (paymentMethod <= 0)
            throw new ArgumentException("Invalid payment method.");

        return new Payment(cardName, cardNumber, expiration, cvv, paymentMethod);
    }
}