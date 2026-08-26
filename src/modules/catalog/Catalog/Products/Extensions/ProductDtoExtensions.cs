

namespace Catalog.Products.Extensions;


// 2. Extension methods for mapping collections seamlessly
public static class ProductDtoExtensions
{
    public static List<ProductDto> ToDtoList(this IEnumerable<Product> products)
    {
        return products.Select(p => p.FromDomain()).ToList();
    }
    public static ProductDto FromDomain(this Product product)
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