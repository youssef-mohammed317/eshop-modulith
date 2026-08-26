using Microsoft.EntityFrameworkCore;

namespace Basket.Features.StoreBasket;

public record StoreBasketCommand(ShoppingCartDto Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class StoreBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch from Cache/DB
        var cart = await repository.GetBasketAsync(command.Cart.UserName, cancellationToken);

        // 2. Create or Clear
        if (cart is null)
        {
            cart = ShoppingCart.Create(command.Cart.Id, command.Cart.UserName);
        }
        else
        {
            cart.ClearCart();
        }

        // 3. Add incoming items
        foreach (var itemDto in command.Cart.Items)
        {
            cart.AddItem(
                itemDto.ProductId,
                itemDto.Quantity,
                itemDto.Color,
                itemDto.Price,
                itemDto.ProductName);
        }

        // 4. Update Cache and DB tracking
        await repository.StoreBasketAsync(cart, cancellationToken);

        // 5. Commit DB transaction
        await repository.SaveChangesAsync(command.Cart.UserName, cancellationToken);

        return new StoreBasketResult(cart.UserName);
    }
}