using Microsoft.EntityFrameworkCore;

namespace Basket.Features.GetBasket;

public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;
public record GetBasketResult(ShoppingCartDto Cart);

public class GetBasketQueryHandler(IBasketRepository repository)
    : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        // Uses the cached repository automatically!
        var cart = await repository.GetBasketAsync(query.UserName, cancellationToken);

        if (cart is null)
        {
            return new GetBasketResult(new ShoppingCartDto(Guid.NewGuid(), query.UserName, new List<ShoppingCartItemDto>()));
        }

        var cartDto = new ShoppingCartDto(
            cart.Id,
            cart.UserName,
            cart.Items.Select(i => new ShoppingCartItemDto(
                i.Id, i.ShoppingCartId, i.ProductId, i.Quantity, i.Color, i.Price, i.ProductName
            )).ToList()
        );

        return new GetBasketResult(cartDto);
    }
}