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

    public async Task<ShoppingCart> CreateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        var existing = await context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserName == basket.UserName, cancellationToken);

        if (existing is not null)
        {
            return await UpdateBasketAsync(basket, cancellationToken);
        }

        context.ShoppingCarts.Add(basket);
        return basket;
    }

    public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        // Load the ONE real tracked instance for this cart, with its current items.
        var trackedCart = await context.ShoppingCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == basket.Id, cancellationToken);

        if (trackedCart is null)
        {
            // Shouldn't normally happen if the caller already confirmed the cart exists,
            // but fall back to a create rather than fail.
            context.ShoppingCarts.Add(basket);
            return basket;
        }

        var incomingItemIds = basket.Items.Select(i => i.Id).ToHashSet();

        // Remove items that are no longer present in the incoming cart state
        foreach (var existingItem in trackedCart.Items.ToList())
        {
            if (!incomingItemIds.Contains(existingItem.Id))
            {
                context.Remove(existingItem);
            }
        }

        // Update matching items, add genuinely new ones
        foreach (var incomingItem in basket.Items)
        {
            var trackedItem = trackedCart.Items.FirstOrDefault(i => i.Id == incomingItem.Id);

            if (trackedItem is not null)
            {
                // Copy scalar values onto the already-tracked instance — no second instance involved
                context.Entry(trackedItem).CurrentValues.SetValues(incomingItem);
            }
            else
            {
                context.Add(incomingItem);
            }
        }

        return trackedCart;
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