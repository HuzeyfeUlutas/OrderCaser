using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Features.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler(
    OrderingDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderResponse>>
{
    public async Task<IReadOnlyList<OrderResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Orders
            .AsNoTracking()
            .Where(o => !o.IsDeleted);

        if (!currentUser.IsAdmin)
            query = query.Where(o => o.UserId == currentUser.UserId);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(OrderResponse.FromEntity).ToList();
    }
}