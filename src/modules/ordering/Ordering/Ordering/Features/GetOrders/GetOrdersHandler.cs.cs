// Location: Ordering/Features/GetOrders/GetOrdersHandler.cs
using Microsoft.EntityFrameworkCore;

namespace Ordering.Features.GetOrders;

public record GetOrdersQuery(PaginatedRequest PaginatedRequest) : IQuery<GetOrdersResult>;
public record GetOrdersResult(PaginatedResult<OrderDto> Orders);

public class GetOrdersQueryHandler(OrderingDbContext dbContext)
    : IQueryHandler<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        // 1. Get total count
        long totalCount = await dbContext.Orders.LongCountAsync(cancellationToken);

        // 2. Fetch paginated data
        var orders = await dbContext.Orders
            .Include(o => o.OrderItems)
            .AsNoTracking()
            .OrderByDescending(o => o.OrderName) // Adjust sorting as needed
            .Skip(query.PaginatedRequest.PageIndex * query.PaginatedRequest.PageSize)
            .Take(query.PaginatedRequest.PageSize)
            .ToListAsync(cancellationToken);

        // 3. Map to DTOs
        var orderDtos = orders.Select(o => o.ToOrderDto()).ToList();

        // 4. Return paginated result
        return new GetOrdersResult(
            new PaginatedResult<OrderDto>(query.PaginatedRequest.PageIndex, query.PaginatedRequest.PageSize, totalCount, orderDtos));
    }
}