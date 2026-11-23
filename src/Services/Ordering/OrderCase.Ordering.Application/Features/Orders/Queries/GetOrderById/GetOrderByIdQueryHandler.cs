using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Application.Common.Exceptions;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    OrderingDbContext dbContext,
    ICacheService cacheService,
    ICurrentUserService currentUser)
    : IRequestHandler<GetOrderByIdQuery, OrderResponse>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = OrderCacheKeys.ById(request.OrderId);

        var response = await cacheService.GetAsync<OrderResponse>(cacheKey, cancellationToken);

        if (response is null)
        {
            var order = await dbContext.Orders
                            .AsNoTracking()
                            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken)
                        ?? throw new NotFoundException($"Order '{request.OrderId}' was not found.");

            response = OrderResponse.FromEntity(order);

            await cacheService.SetAsync(cacheKey, response, CacheExpiration, cancellationToken);
        }

        if (!currentUser.IsAdmin && response.UserId != currentUser.UserId)
            throw new ForbiddenException("You are not allowed to view this order.");

        return response;
    }
}