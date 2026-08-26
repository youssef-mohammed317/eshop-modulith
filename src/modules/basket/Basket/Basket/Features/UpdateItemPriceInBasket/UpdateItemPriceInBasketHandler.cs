using Microsoft.EntityFrameworkCore;

namespace Basket.Features.UpdateItemPriceInBasket;

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
    IBasketRepository repository)
    : ICommandHandler<UpdateItemPriceInBasketCommand, UpdateItemPriceInBasketResult>
{
    public async Task<UpdateItemPriceInBasketResult> Handle(
        UpdateItemPriceInBasketCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Query all carts containing the product
        var carts = await dbContext.ShoppingCarts
            .Include(c => c.Items)
            .Where(c => c.Items.Any(i => i.ProductId == command.ProductId))
            .ToListAsync(cancellationToken);

        if (carts.Count == 0)
        {
            return new UpdateItemPriceInBasketResult(true);
        }

        // 2. Update each cart domain model and sync with cache
        foreach (var cart in carts)
        {
            cart.UpdateItemPrice(command.ProductId, command.Price);
            await repository.StoreBasketAsync(cart, cancellationToken);
        }

        // 3. Persist database changes
        await repository.SaveChangesAsync(null, cancellationToken);

        return new UpdateItemPriceInBasketResult(true);
    }
}
