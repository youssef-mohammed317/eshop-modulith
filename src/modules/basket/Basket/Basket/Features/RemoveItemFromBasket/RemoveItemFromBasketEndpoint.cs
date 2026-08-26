namespace Basket.Features.RemoveItemFromBasket;

//public record RemoveItemFromBasketRequest(Guid ProductId);
public record RemoveItemFromBasketResponse(bool IsSuccess);

public class RemoveItemFromBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Using both userName and productId in the route makes this perfectly RESTful
        app.MapDelete("/basket/{userName}/items/{productId}", async (string userName, Guid productId, ISender sender) =>
        {
            var command = new RemoveItemFromBasketCommand(userName, productId);

            var result = await sender.Send(command);

            var response = result.Adapt<RemoveItemFromBasketResponse>();

            return Results.Ok(response);
        })
        .WithName("RemoveItemFromBasket")
        .Produces<RemoveItemFromBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Remove Item From Basket")
        .WithDescription("Removes a specific product from a user's shopping cart")
        .WithTags("Basket");
    }
}