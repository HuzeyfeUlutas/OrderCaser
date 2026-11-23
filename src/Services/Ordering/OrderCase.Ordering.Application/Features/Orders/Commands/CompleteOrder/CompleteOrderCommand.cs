using MediatR;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Common.Requests;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.CompleteOrder;

public sealed record CompleteOrderCommand(Guid OrderId)
    : IRequest, ITransactionalRequest, ICacheInvalidatingRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => [OrderCacheKeys.ById(OrderId)];
}