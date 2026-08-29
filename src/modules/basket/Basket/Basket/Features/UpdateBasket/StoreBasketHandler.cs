namespace Basket.Basket.Features.UpdateBasket;

public record UpdateBasketCommand(ShoppingCartDto Cart) : ICommand<UpdateBasketResult>;

public record UpdateBasketResult(bool IsSuccess);

public class UpdateBasketCommandValidator : AbstractValidator<UpdateBasketCommand>
{
    public UpdateBasketCommandValidator()
    {
        RuleFor(x => x.Cart.Id).NotEmpty().WithMessage("Cart Id is required");
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

public class UpdateBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<UpdateBasketCommand, UpdateBasketResult>
{
    public async Task<UpdateBasketResult> Handle(UpdateBasketCommand command, CancellationToken cancellationToken)
    {
        var existing = await repository.GetBasketAsync(command.Cart.UserName, cancellationToken);
        if (existing is null)
        {
            throw new BasketNotFoundException(command.Cart.UserName);
        }

        var cart = ShoppingCart.Create(command.Cart.Id, command.Cart.UserName);

        foreach (var item in command.Cart.Items)
        {
            // Items with a real Id already exist \u2014 preserve them via LoadItem.
            // Items with an empty Id are genuinely new \u2014 AddItem mints a fresh Id for them.
            if (item.Id != Guid.Empty)
            {
                cart.LoadItem(item.Id, item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
            }
            else
            {
                cart.AddItem(item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
            }
        }

        await repository.UpdateBasketAsync(cart, cancellationToken);
        await repository.SaveChangesAsync(command.Cart.UserName, cancellationToken);

        return new UpdateBasketResult(true);
    }
}