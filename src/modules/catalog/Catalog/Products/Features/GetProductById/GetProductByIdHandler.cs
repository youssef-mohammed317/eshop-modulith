using Microsoft.EntityFrameworkCore;

namespace Catalog.Products.Features.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdResult>;

public record GetProductByIdResult(ProductDto Product);

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product Id is required");
    }
}

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
            throw new Exception($"Product with Id {query.Id} not found");
        }

        // Map single entity using the static factory method
        var productDto = ProductDto.FromDomain(product);

        return new GetProductByIdResult(productDto);
    }
}