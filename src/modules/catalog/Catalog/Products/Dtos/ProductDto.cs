namespace Catalog.Products.Dtos;

// 1. For returning data to the client (Querying)
public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string ImageFile,
    List<string> Category,
    decimal Price
)
{
    // Maps a single Product domain entity to a ProductDto
    public static ProductDto FromDomain(Product product)
    {
        return new ProductDto(
            Id: product.Id,
            Name: product.Name,
            Description: product.Description,
            ImageFile: product.ImageFile,
            // .ToList() is necessary if your domain model exposes Category as IReadOnlyList<string>
            Category: product.Category.ToList(),
            Price: product.Price
        );
    }
}

// 2. Extension methods for mapping collections seamlessly
public static class ProductDtoExtensions
{
    public static List<ProductDto> ToDtoList(this IEnumerable<Product> products)
    {
        return products.Select(ProductDto.FromDomain).ToList();
    }
}