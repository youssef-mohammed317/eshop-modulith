using System.Text.Json;
using Basket.Basket.Models;
using Basket.Features;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Data.Repository;

public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        var cachedString = await cache.GetStringAsync(userName, cancellationToken);

        if (!string.IsNullOrEmpty(cachedString))
        {
            var dto = JsonSerializer.Deserialize<ShoppingCartDto>(cachedString);

            if (dto is not null)
            {
                var cachedCart = ShoppingCart.Create(dto.Id, dto.UserName);

                foreach (var item in dto.Items)
                {
                    // LoadItem preserves the item's real Id, instead of AddItem's
                    // behavior of always minting a brand-new random one.
                    cachedCart.LoadItem(item.Id, item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
                }

                return cachedCart;
            }
        }

        var basket = await repository.GetBasketAsync(userName, cancellationToken);

        if (basket is not null)
        {
            await CacheBasketAsync(basket, cancellationToken);
        }

        return basket;
    }

    public async Task<ShoppingCart> CreateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        return await repository.CreateBasketAsync(basket, cancellationToken);
    }

    public async Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        return await repository.UpdateBasketAsync(basket, cancellationToken);
    }

    public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        var success = await repository.DeleteBasketAsync(userName, cancellationToken);

        if (success)
        {
            await cache.RemoveAsync(userName, cancellationToken);
        }

        return success;
    }

    public async Task SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
    {
        await repository.SaveChangesAsync(userName, cancellationToken);
        if (userName != null)
            await cache.RemoveAsync(userName, cancellationToken);
    }

    private async Task CacheBasketAsync(ShoppingCart basket, CancellationToken cancellationToken)
    {
        var dto = new ShoppingCartDto(
            basket.Id,
            basket.UserName,
            basket.Items.Select(i => new ShoppingCartItemDto(
                i.Id, i.ShoppingCartId, i.ProductId, i.Quantity, i.Color, i.Price, i.ProductName
            )).ToList()
        );

        var json = JsonSerializer.Serialize(dto);
        await cache.SetStringAsync(basket.UserName, json, cancellationToken);
    }
}