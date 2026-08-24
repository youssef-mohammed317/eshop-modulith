using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery(PaginatedRequest PaginatedRequest) : IQuery<GetProductsResult>;

public record GetProductsResult(PaginatedResult<ProductDto> PaginatedResult);


// 2. Inject CatalogDbContext instead of IDocumentSession
public class GetProductsQueryHandler(CatalogDbContext context)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var totalCount = await context.Products.LongCountAsync();
        // 3. EF Core pagination logic
        var products = await context.Products
            .AsNoTracking() // Crucial for performance: tells EF Core not to track these entities for updates
            .Skip((query.PaginatedRequest.PageIndex) * query.PaginatedRequest.PageSize)
            .Take(query.PaginatedRequest.PageSize)
            .ToListAsync(cancellationToken);

        // 4. Map to DTOs using the extension method we created earlier
        var productDtos = products.ToDtoList();

        return new GetProductsResult(
            new PaginatedResult<ProductDto>(
                query.PaginatedRequest.PageIndex,
                query.PaginatedRequest.PageSize,
                totalCount, productDtos));
    }
}