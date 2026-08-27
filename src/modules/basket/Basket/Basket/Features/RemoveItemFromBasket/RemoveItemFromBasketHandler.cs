using Microsoft.EntityFrameworkCore;

namespace Basket.Basket.Features.AddItemToBasket;

public record RemoveItemFromBasketCommand(string UserName, Guid ProductId) : ICommand<RemoveItemFromBasketResult>;
public record RemoveItemFromBasketResult(bool IsSuccess);

public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
    public RemoveItemFromBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
    }
}

public class RemoveItemFromBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<RemoveItemFromBasketCommand, RemoveItemFromBasketResult>
{
    public async Task<RemoveItemFromBasketResult> Handle(RemoveItemFromBasketCommand command, CancellationToken cancellationToken)
    {
        // 1. Fetch from Cache/DB
        var cart = await repository.GetBasketAsync(command.UserName, cancellationToken);

        if (cart is null)
        {
            throw new BasketNotFoundException(command.UserName);
        }

        // 2. Remove the item from the Domain aggregate
        cart.RemoveItem(command.ProductId);

        // 3. CRUCIAL: Call StoreBasketAsync to overwrite the Redis cache with the updated cart state
        await repository.StoreBasketAsync(cart, cancellationToken);

        // 4. Commit DB transaction (EF Core change tracker handles the item deletion)
        await repository.SaveChangesAsync(command.UserName, cancellationToken);

        return new RemoveItemFromBasketResult(true);
    }
}