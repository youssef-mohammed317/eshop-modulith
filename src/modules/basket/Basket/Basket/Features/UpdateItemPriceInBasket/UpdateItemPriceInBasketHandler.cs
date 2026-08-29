using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Basket.Features.AddItemToBasket;

public record UpdateItemPriceInBasketCommand(Guid ProductId, decimal Price)
    : ICommand<UpdateItemPriceInBasketResult>;

public record UpdateItemPriceInBasketResult(bool IsSuccess);

public class UpdateItemPriceInBasketCommandValidator : AbstractValidator<UpdateItemPriceInBasketCommand>
{
    public UpdateItemPriceInBasketCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be greater than or equal to zero");
    }
}


public class UpdateItemPriceInBasketCommandHandler(
    BasketDbContext dbContext,
    IDistributedCache cache)
    : ICommandHandler<UpdateItemPriceInBasketCommand, UpdateItemPriceInBasketResult>
{
    public async Task<UpdateItemPriceInBasketResult> Handle(
        UpdateItemPriceInBasketCommand command,
        CancellationToken cancellationToken)
    {
        var carts = await dbContext.ShoppingCarts
            .Include(c => c.Items)
            .Where(c => c.Items.Any(i => i.ProductId == command.ProductId))
            .ToListAsync(cancellationToken);

        if (carts.Count == 0)
        {
            return new UpdateItemPriceInBasketResult(true);
        }

        foreach (var cart in carts)
        {
            cart.UpdateItemPrice(command.ProductId, command.Price);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Invalidate each affected user's cache entry individually
        foreach (var cart in carts)
        {
            await cache.RemoveAsync(cart.UserName, cancellationToken);
        }

        return new UpdateItemPriceInBasketResult(true);
    }
}