using Catalog.Products.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Products.Features.GetProductsByCategory;

public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;

public record GetProductsByCategoryResult(IEnumerable<ProductDto> Products);

public class GetProductsByCategoryQueryValidator : AbstractValidator<GetProductsByCategoryQuery>
{
    public GetProductsByCategoryQueryValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Product Category is required");
    }
}

public class GetProductsByCategoryQueryHandler(CatalogDbContext context)
    : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
{
    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        // EF Core 8 natively translates this .Contains() to a PostgreSQL Array search (ANY)
        var products = await context.Products
            .AsNoTracking()
            .Where(p => p.Category.Contains(query.Category))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        // Map to DTOs
        var productDtos = products.ToDtoList();

        return new GetProductsByCategoryResult(productDtos);
    }
}