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
        var cart = await repository.GetBasketAsync(command.UserName, cancellationToken);

        if (cart is null)
        {
            throw new BasketNotFoundException(command.UserName);
        }

        cart.RemoveItem(command.ProductId);

        await repository.UpdateBasketAsync(cart, cancellationToken);
        await repository.SaveChangesAsync(command.UserName, cancellationToken);

        return new RemoveItemFromBasketResult(true);
    }
}