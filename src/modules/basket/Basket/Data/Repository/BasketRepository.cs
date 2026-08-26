using Microsoft.EntityFrameworkCore;

namespace Basket.Data.Repository;

public class BasketRepository(BasketDbContext context) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserName == userName, cancellationToken);
    }

    public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        var existingBasket = await GetBasketAsync(basket.UserName, cancellationToken);

        if (existingBasket is null)
        {
            context.ShoppingCarts.Add(basket);
        }
        else
        {
            context.ShoppingCarts.Update(basket);
        }
        return basket;
    }

    public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        var basket = await GetBasketAsync(userName, cancellationToken);

        if (basket is not null)
        {
            context.ShoppingCarts.Remove(basket);
            return true;
        }
        return false;
    }

    public async Task SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}