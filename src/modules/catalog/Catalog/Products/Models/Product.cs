namespace Catalog.Products.Models;

public class Product : Aggregate<Guid>
{
    // 1. Private setters prevent external, uncontrolled changes
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string ImageFile { get; private set; } = default!;
    public decimal Price { get; private set; }

    // 2. Encapsulate collections so elements can't be added/removed without domain logic
    private readonly List<string> _categories = new();
    public IReadOnlyList<string> Category => _categories.AsReadOnly();

    // 3. Parameterless constructor required by ORMs like EF Core (kept hidden)
    protected Product() { }

    // Private constructor for internal instantiation
    private Product(Guid id, string name, string description, string imageFile, decimal price)
    {
        Id = id;
        Name = name;
        Description = description;
        ImageFile = imageFile;
        Price = price;
    }

    // 4. Static factory method to handle creation and enforce invariants
    public static Product Create(Guid id, string name, string description, string imageFile, decimal price)
    {
        // Guard clauses to ensure the entity is always in a valid state
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        var product = new Product(id, name, description, imageFile, price);

        // Example: product.AddDomainEvent(new ProductCreatedEvent(product.Id));
        product.AddDomainEvent(new ProductCreatedEvent(product));
        return product;
    }

    // 5. Explicit behavior methods (Ubiquitous Language) instead of raw setters
    public void UpdateDetails(string name, string description, string imageFile)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        Name = name;
        Description = description;
        ImageFile = imageFile;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(newPrice));

        Price = newPrice;

        // Example: AddDomainEvent(new ProductPriceChangedEvent(Id, newPrice));
        this.AddDomainEvent(new ProductPriceChangedEvent(this));
    }

    public void AddCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty.", nameof(category));

        if (!_categories.Contains(category))
        {
            _categories.Add(category);
        }
    }

    public void RemoveCategory(string category)
    {
        _categories.Remove(category);
    }
}