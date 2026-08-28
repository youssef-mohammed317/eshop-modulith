namespace Catalog.Products.Features.GetProducts;

//public record GetProductsRequest(PaginatedRequest PaginatedRequest);
public record GetProductsResponse(PaginatedResult<ProductDto> PaginatedResult);

public class GetProductsEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products",
            async ([AsParameters] PaginatedRequest request, ISender sender) =>
            {
                var query = new GetProductsQuery(request);

                var result = await sender.Send(query);

                var response = result.Adapt<GetProductsResponse>();

                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Products")
            .WithDescription("Get Products")
            .WithTags("Products"); // Groups endpoints together in the Swagger UI
    }
}
