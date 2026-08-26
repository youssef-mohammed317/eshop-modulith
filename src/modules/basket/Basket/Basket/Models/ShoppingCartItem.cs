using System;
using Shared.DDD;

namespace Basket.Basket.Models;

public class ShoppingCartItem : Entity<Guid>
{
    public Guid ShoppingCartId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public string Color { get; private set; } = default!;
    public decimal Price { get; private set; }
    public string ProductName { get; private set; } = default!;

    // 1. Parameterless constructor required by EF Core / Redis deserialization
    protected ShoppingCartItem() { }

    internal ShoppingCartItem(Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
    {
        Id = Guid.NewGuid(); // Initialize the primary key!
        ShoppingCartId = shoppingCartId;
        ProductId = productId;
        Quantity = quantity;
        Color = color;
        Price = price;
        ProductName = productName;
    }

    // 2. Encapsulated behavior methods (Better than 'internal set')
    internal void IncreaseQuantity(int quantity)
    {
        Quantity += quantity;
    }

    internal void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be at least 1.");

        Quantity = newQuantity;
    }
}