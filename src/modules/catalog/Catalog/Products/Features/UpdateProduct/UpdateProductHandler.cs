using Catalog.Contracts.Products.Exceptions;

namespace Catalog.Products.Features.UpdateProduct;

public record UpdateProductCommand(ProductDto ProductDto)
    : ICommand<UpdateProductResult>;
public record UpdateProductResult(bool IsSuccess);


public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductDto.Id)
            .NotEmpty().WithMessage("Product Id is required");

        RuleFor(x => x.ProductDto.Name)
            .NotEmpty().WithMessage("Product Name is required");

        RuleFor(x => x.ProductDto.Category)
            .NotEmpty().WithMessage("Product Category is required");

        RuleFor(x => x.ProductDto.Description)
            .NotEmpty().WithMessage("Product Description is required");

        RuleFor(x => x.ProductDto.ImageFile)
            .NotEmpty().WithMessage("Product ImageFile is required");

        RuleFor(x => x.ProductDto.Price)
            .GreaterThan(0).WithMessage("Product Price must be greater than zero");
    }
}



public class UpdateProductCommandHandler(CatalogDbContext context)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var productDto = command.ProductDto;

        // 1. Load the product from the database
        var product = await context.Products.FindAsync(new object[] { productDto.Id }, cancellationToken);

        if (product is null)
        {
            // Ideally, throw a custom NotFoundException here so a global exception handler can return a 404
            throw new ProductNotFoundException(productDto.Id);
        }

        // 2. Update properties using the Rich Domain behavior methods (preserves encapsulation)
        product.UpdateDetails(productDto.Name, productDto.Description, productDto.ImageFile);
        product.UpdatePrice(productDto.Price);

        // 3. Update categories (Clear existing, then add new ones)
        product.ReplaceCategories(productDto.Category);

        // 4. Save changes
        context.Products.Update(product); // Explicit update tracking
        await context.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}