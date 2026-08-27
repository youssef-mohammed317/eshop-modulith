// Location: Ordering/Features/GetOrderById/GetOrderByIdHandler.cs
using Microsoft.EntityFrameworkCore;

namespace Ordering.Features.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IQuery<GetOrderByIdResult>;
public record GetOrderByIdResult(OrderDto Order);

public class GetOrderByIdQueryHandler(OrderingDbContext dbContext)
    : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResult>
{
    public async Task<GetOrderByIdResult> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .Include(o => o.OrderItems)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == query.OrderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(query.OrderId);
        }

        return new GetOrderByIdResult(order.ToOrderDto());
    }
}