namespace Ordering.Ordering.ValueObjects;

public record Address
{
    public string FirstName { get; } = default!;
    public string LastName { get; } = default!;
    public string EmailAddress { get; } = default!;
    public string AddressLine { get; } = default!;
    public string Country { get; } = default!;
    public string State { get; } = default!;
    public string ZipCode { get; } = default!;

    // Protected parameterless constructor required by Entity Framework Core
    protected Address() { }

    // Private constructor to force the use of the factory method
    private Address(string firstName, string lastName, string emailAddress, string addressLine, string country, string state, string zipCode)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        AddressLine = addressLine;
        Country = country;
        State = state;
        ZipCode = zipCode;
    }

    /// <summary>
    /// Factory method to create a new Address instance with validation.
    /// </summary>
    public static Address Of(string firstName, string lastName, string emailAddress, string addressLine, string country, string state, string zipCode)
    {
        // Validation rules using ArgumentException
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Email address cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(addressLine))
            throw new ArgumentException("Address line cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("Zip code cannot be null or empty.");

        return new Address(firstName, lastName, emailAddress, addressLine, country, state, zipCode);
    }
}