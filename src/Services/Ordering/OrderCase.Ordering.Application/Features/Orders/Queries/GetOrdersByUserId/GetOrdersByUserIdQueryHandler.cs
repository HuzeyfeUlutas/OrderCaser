using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Application.Common.Exceptions;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Features.Orders.Queries.GetOrdersByUserId;

public sealed class GetOrdersByUserIdQueryHandler(
    OrderingDbContext dbContext,
    ICacheService cacheService,
    ICurrentUserService currentUser)
    : IRequestHandler<GetOrdersByUserIdQuery, IReadOnlyList<OrderResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(2);

    public async Task<IReadOnlyList<OrderResponse>> Handle(
        GetOrdersByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAdmin && request.UserId != currentUser.UserId)
            throw new ForbiddenException("You are not allowed to view another user's orders.");

        var cacheKey = OrderCacheKeys.ByUser(request.UserId);

        var cached = await cacheService.GetAsync<List<OrderResponse>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var orders = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == request.UserId && !o.IsDeleted)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        var response = orders.Select(OrderResponse.FromEntity).ToList();

        await cacheService.SetAsync(cacheKey, response, CacheExpiration, cancellationToken);

        return response;
    }
}