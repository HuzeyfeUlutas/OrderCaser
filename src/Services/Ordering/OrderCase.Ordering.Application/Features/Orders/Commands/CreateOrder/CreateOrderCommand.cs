using MediatR;
using OrderCase.Ordering.Application.Abstractions.Caching;
using OrderCase.Ordering.Application.Common.Requests;
using OrderCase.Ordering.Domain.Orders;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    string UserId,
    string ProductId,
    int Quantity,
    PaymentMethod PaymentMethod)
    : IRequest<CreateOrderResult>, ITransactionalRequest, ICacheInvalidatingRequest
{
    public IEnumerable<string> CacheKeysToInvalidate => [OrderCacheKeys.ByUser(UserId)];
}