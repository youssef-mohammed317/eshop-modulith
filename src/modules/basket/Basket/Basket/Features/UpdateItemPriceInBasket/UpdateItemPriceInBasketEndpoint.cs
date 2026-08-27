namespace Basket.Basket.Features.AddItemToBasket;

public record UpdateItemPriceInBasketRequest(decimal Price);
public record UpdateItemPriceInBasketResponse(bool IsSuccess);


public class UpdateItemPriceInBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/basket/items/{productId}/price", async (
            Guid productId,
            UpdateItemPriceInBasketRequest request,
            ISender sender) =>
        {
            var command = new UpdateItemPriceInBasketCommand(productId, request.Price);
            var result = await sender.Send(command);

            return Results.Ok(new UpdateItemPriceInBasketResponse(result.IsSuccess));
        })
        .WithName("UpdateItemPriceInBasket")
        .Produces<UpdateItemPriceInBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update Item Price in Basket")
        .WithDescription("Updates the price of a specific product across all active shopping carts")
        .WithTags("Basket");
    }
}