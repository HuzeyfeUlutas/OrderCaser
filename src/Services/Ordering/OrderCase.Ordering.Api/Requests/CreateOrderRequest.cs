using OrderCase.Ordering.Domain.Orders;

namespace OrderCase.Ordering.Api.Requests;

public sealed record CreateOrderRequest(
    string UserId,
    string ProductId,
    int Quantity,
    PaymentMethod PaymentMethod);