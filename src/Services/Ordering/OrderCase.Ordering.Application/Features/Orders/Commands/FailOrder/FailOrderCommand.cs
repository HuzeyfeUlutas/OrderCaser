using MediatR;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Common.Requests;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.FailOrder;

public sealed record FailOrderCommand(Guid OrderId, string Reason)
    : IRequest, ITransactionalRequest, ICacheInvalidatingRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => [OrderCacheKeys.ById(OrderId)];
}