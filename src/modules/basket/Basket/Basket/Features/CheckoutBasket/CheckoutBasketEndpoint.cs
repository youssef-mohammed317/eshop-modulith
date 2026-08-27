// Location: Basket/Features/CheckoutBasket/CheckoutBasketEndpoint.cs
namespace Basket.Features.CheckoutBasket;

public class CheckoutBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/checkout", async (BasketCheckoutDto request, ISender sender) =>
        {
            var command = new CheckoutBasketCommand(request);

            var result = await sender.Send(command);

            if (!result.IsSuccess)
            {
                return Results.BadRequest("Basket not found or checkout failed.");
            }

            // Return 202 Accepted because the actual order creation happens asynchronously via RabbitMQ
            return Results.Accepted();
        })
        .WithName("CheckoutBasket")
        .Produces(StatusCodes.Status202Accepted)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Checkout Basket")
        .WithDescription("Initiates the checkout process by publishing a BasketCheckoutEvent")
        .WithTags("Basket");
    }
}