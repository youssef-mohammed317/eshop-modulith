public class ShoppingCartItem : Entity<Guid>
{
    public Guid ShoppingCartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string Color { get; private set; } = default!;
    public decimal Price { get; private set; }
    public string ProductName { get; private set; } = default!;

    protected ShoppingCartItem() { }

    internal ShoppingCartItem(Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
        : this(Guid.NewGuid(), shoppingCartId, productId, quantity, color, price, productName)
    {
    }

    internal ShoppingCartItem(Guid id, Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
    {
        Id = id;
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Quantity = quantity;
        Color = color;
        Price = price;
        ProductName = productName;
    }

    internal void IncreaseQuantity(int quantity) => Quantity += quantity;

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be at least 1.");
        Quantity = newQuantity;
    }

    internal void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("newPrice cant be negative");
        Price = newPrice;
    }
}