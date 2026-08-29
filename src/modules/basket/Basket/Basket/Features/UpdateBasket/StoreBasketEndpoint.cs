using Microsoft.AspNetCore.Mvc;

namespace Basket.Basket.Features.UpdateBasket;

public record UpdateBasketRequest(ShoppingCartDto Cart);
public record UpdateBasketResponse(bool IsSuccess);

public class UpdateBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/basket", async ([FromBody] UpdateBasketRequest request, ISender sender) =>
        {
            var command = new UpdateBasketCommand(request.Cart);
            var result = await sender.Send(command);
            return Results.Ok(new UpdateBasketResponse(result.IsSuccess));
        })
        .WithName("UpdateBasket")
        .Produces<UpdateBasketResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Basket")
        .WithDescription("Updates an existing shopping basket's items")
        .RequireAuthorization();
    }
}