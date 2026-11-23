using MediatR;

namespace OrderCase.Ordering.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderResponse>;