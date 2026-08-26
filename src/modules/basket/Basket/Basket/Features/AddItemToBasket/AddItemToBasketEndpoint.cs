// Add your DTO namespace here

namespace Basket.Features.AddItemToBasket;

// The request body only needs to contain the item details
public record AddItemToBasketRequest(ShoppingCartItemDto ShoppingCartItem);
public record AddItemToBasketResponse(Guid CartId);

public class AddItemToBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/{userName}/items", async (string userName, AddItemToBasketRequest request, ISender sender) =>
        {
            // Combine the route parameter and the request body into your Command
            var command = new AddItemToBasketCommand(userName, request.ShoppingCartItem);

            var result = await sender.Send(command);

            var response = new AddItemToBasketResponse(result.CartId);

            return Results.Created($"/basket/{userName}", response);
        })
        .WithName("AddItemToBasket")
        .Produces<AddItemToBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Add Item To Basket")
        .WithDescription("Adds a single item to a user's shopping cart")
        .WithTags("Basket");
    }
}