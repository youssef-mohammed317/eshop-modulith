// Location: Ordering/Features/GetOrders/GetOrdersEndpoint.cs
namespace Ordering.Features.GetOrders;

//public record GetOrdersRequest(PaginatedRequest PaginatedRequest);
public record GetOrdersResponse(PaginatedResult<OrderDto> Orders);

public class GetOrdersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/orders", async ([AsParameters] PaginatedRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetOrdersQuery(request));
            return Results.Ok(new GetOrdersResponse(result.Orders));
        })
        .WithName("GetOrders")
        .Produces<GetOrdersResponse>(StatusCodes.Status200OK);
    }
}
