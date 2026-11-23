using MediatR;

namespace OrderCase.Ordering.Application.Features.Orders.Queries.GetOrdersByUserId;

public sealed record GetOrdersByUserIdQuery(string UserId) : IRequest<IReadOnlyList<OrderResponse>>;