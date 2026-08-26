using System.Text.Json;
using Basket.Basket.Models; // Domain models
using Basket.Features; // Wherever your DTOs live
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.Data.Repository;

public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        var cachedString = await cache.GetStringAsync(userName, cancellationToken);

        if (!string.IsNullOrEmpty(cachedString))
        {
            // 1. Deserialize into the DTO, which System.Text.Json can handle easily
            var dto = JsonSerializer.Deserialize<ShoppingCartDto>(cachedString);

            if (dto is not null)
            {
                // 2. Reconstruct the Rich Domain Model using your encapsulated methods
                var cachedCart = ShoppingCart.Create(dto.Id, dto.UserName);

                foreach (var item in dto.Items)
                {
                    cachedCart.AddItem(item.ProductId, item.Quantity, item.Color, item.Price, item.ProductName);
                }

                return cachedCart;
            }
        }

        // Cache miss: go to the database
        var basket = await repository.GetBasketAsync(userName, cancellationToken);

        if (basket is not null)
        {
            // Map Domain to DTO before caching
            await CacheBasketAsync(basket, cancellationToken);
        }

        return basket;
    }

    public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        // 1. Save to Database via the EF Core Repository
        var updatedBasket = await repository.StoreBasketAsync(basket, cancellationToken);

        // 2. Update the cache using the DTO
        await CacheBasketAsync(updatedBasket, cancellationToken);

        return updatedBasket;
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

    // Helper method to handle the Domain -> DTO -> Json string mapping
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