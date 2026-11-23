using MediatR;

namespace OrderCase.Ordering.Application.Features.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery : IRequest<IReadOnlyList<OrderResponse>>;