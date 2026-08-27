namespace Ordering.Ordering.Models;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    // Protected parameterless constructor for EF Core
    protected OrderItem() { }

    // Internal constructor ensures ONLY the Order aggregate can instantiate this
    internal OrderItem(Guid orderId, Guid productId, int quantity, decimal price)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    // Internal factory method with invariant validation
    internal static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.");

        return new OrderItem(orderId, productId, quantity, price);
    }

    // Behavior method for the Aggregate Root to use
    internal void AddQuantity(int additionalQuantity)
    {
        if (additionalQuantity <= 0)
            throw new ArgumentException("Additional quantity must be greater than zero.");

        Quantity += additionalQuantity;
    }
}