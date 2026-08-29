using Microsoft.AspNetCore.Mvc;

namespace Basket.Basket.Features.CreateBasket;

public record CreateBasketRequest(ShoppingCartDto Cart);
public record CreateBasketResponse(Guid Id);

public class CreateBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async ([FromBody] CreateBasketRequest request, ISender sender) =>
        {
            var command = new CreateBasketCommand(request.Cart);
            var result = await sender.Send(command);
            var response = new CreateBasketResponse(result.Id);
            return Results.Created($"/basket/{request.Cart.UserName}", response);
        })
        .WithName("CreateBasket")
        .Produces<CreateBasketResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Basket")
        .WithDescription("Creates a new shopping basket for a user")
        .RequireAuthorization();
    }
}