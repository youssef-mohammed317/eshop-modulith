using Catalog.Contracts.Products.Exceptions;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Catalog.Products.Features.GetProductById;

//public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

//public record GetProductByIdResult(ProductDto Product);

public class GetProductByIdQueryHandler(CatalogDbContext context)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        // Load tracking-free entity directly from PostgreSQL
        var product = await context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(query.Id);
        }

        // Map single entity using the static factory method
        var productDto = product.FromDomain();

        return new GetProductByIdResult(productDto);
    }
}