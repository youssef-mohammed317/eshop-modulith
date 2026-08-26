namespace Basket.Basket.Models;

public class ShoppingCart : Aggregate<Guid>
{
    // 1. Private setters prevent external tampering
    public string UserName { get; private set; } = default!;

    // 2. Encapsulate the collection
    private readonly List<ShoppingCartItem> _items = new();
    public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();

    // Calculated property (Read-only by design)
    public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

    // 3. Parameterless constructor hidden for EF Core
    protected ShoppingCart() { }

    private ShoppingCart(Guid id, string userName)
    {
        Id = id;
        UserName = userName;
    }

    // 4. Static Factory Method
    public static ShoppingCart Create(Guid id, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username is required.", nameof(userName));

        return new ShoppingCart(id, userName);
    }

    // 5. Behavior Methods (The core of a Rich Domain)
    public void AddItem(Guid productId, int quantity, string color, decimal price, string productName)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem != null)
        {
            // If the item is already in the cart, just increase the quantity
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            // Otherwise, add a new item
            var newItem = new ShoppingCartItem(this.Id, productId, quantity, color, price, productName);
            _items.Add(newItem);
        }
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }

    public void ClearCart()
    {
        _items.Clear();
    }

    // Inside Basket.Basket.Models.ShoppingCart
    public void UpdateItemPrice(Guid productId, decimal newPrice)
    {
        var item = _items.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            item.UpdatePrice(newPrice); // Ensure you add this internal method to ShoppingCartItem!
        }
    }
}