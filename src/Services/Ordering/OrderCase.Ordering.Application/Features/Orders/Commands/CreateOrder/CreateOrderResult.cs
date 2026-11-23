namespace OrderCase.Ordering.Application.Features.Orders.Commands.CreateOrder;

public sealed record CreateOrderResult(Guid OrderId, string Status);