using Catalog.Contracts.Products.Exceptions;

namespace Catalog.Products.Features.DeleteProduct;

public record DeleteProductCommand(Guid Id)
    : ICommand<DeleteProductResult>;
public record DeleteProductResult(bool IsSuccess);


public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product Id is required");
    }
}



public class DeleteProductCommandHandler(CatalogDbContext context)
    : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        // 1. Find the entity
        var product = await context.Products.FindAsync(new object[] { command.Id }, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Id);
        }

        // 2. Remove it from tracking
        context.Products.Remove(product);

        // 3. Commit to database
        await context.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}