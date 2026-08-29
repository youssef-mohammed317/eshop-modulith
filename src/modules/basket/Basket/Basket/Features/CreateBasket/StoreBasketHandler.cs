namespace Basket.Basket.Features.CreateBasket;

public record CreateBasketCommand(ShoppingCartDto Cart) : ICommand<CreateBasketResult>;

public record CreateBasketResult(Guid Id);

public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
{
    public CreateBasketCommandValidator()
    {
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
        RuleForEach(x => x.Cart.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("ProductId is required");
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
            item.RuleFor(i => i.Price).GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative");
            item.RuleFor(i => i.ProductName).NotEmpty().WithMessage("ProductName is required");
        });
    }
}

public class CreateBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<CreateBasketCommand, CreateBasketResult>
{
    public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
    {
        var existing = await repository.GetBasketAsync(command.Cart.UserName, cancellationToken);
        if (existing is not null)
        {
            throw new BasketAlreadyExistsException(command.Cart.UserName);
        }

        var cartId = command.Cart.Id == Guid.Empty ? Guid.NewGuid() : command.Cart.Id;
        var cart = ShoppingCart.Create(cartId, command.Cart.UserName);

        foreach (var item in command.Cart.Items)
        {
            cart.AddItem(item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
        }

        await repository.CreateBasketAsync(cart, cancellationToken);
        await repository.SaveChangesAsync(command.Cart.UserName, cancellationToken);

        return new CreateBasketResult(cart.Id);
    }
}