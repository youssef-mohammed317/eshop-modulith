namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto ProductDto)
    : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
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


public class CreateProductCommandHandler(CatalogDbContext context)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // 1. Create the rich domain entity
        var product = CreateNewProduct(command.ProductDto);

        // 2. Add the entity to EF Core tracking
        context.Products.Add(product);

        // 3. Save to database (your interceptors will handle audit fields & domain events)
        await context.SaveChangesAsync(cancellationToken);

        // 4. Return the newly generated ID
        return new CreateProductResult(product.Id);
    }

    private Product CreateNewProduct(ProductDto productDto)
    {
        // 1. Instantiate using your static factory method and generate a new ID
        var product = Product.Create(
            id: Guid.NewGuid(),
            name: productDto.Name,
            description: productDto.Description,
            imageFile: productDto.ImageFile,
            price: productDto.Price
        );

        // 2. Add categories using the encapsulated behavior method
        if (productDto.Category != null)
        {
            foreach (var category in productDto.Category)
            {
                product.AddCategory(category);
            }
        }

        return product;
    }
}