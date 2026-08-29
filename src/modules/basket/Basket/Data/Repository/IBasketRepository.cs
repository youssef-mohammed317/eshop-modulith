namespace Basket.Data.Repository;

public interface IBasketRepository
{
    Task<ShoppingCart?> GetBasketAsync(string userName, CancellationToken cancellationToken = default);
    Task<ShoppingCart> CreateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default);
    Task<ShoppingCart> UpdateBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default);
    Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default);
}