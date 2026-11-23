using OrderCase.Ordering.Domain.Orders;

namespace OrderCase.Ordering.Application.Features.Orders;

public sealed record OrderResponse(
    Guid Id,
    string UserId,
    string ProductId,
    int Quantity,
    string PaymentMethod,
    string Status,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public static OrderResponse FromEntity(Order order) => new(
        order.Id,
        order.UserId,
        order.ProductId,
        order.Quantity,
        order.PaymentMethod.ToString(),
        order.Status.ToString(),
        order.FailureReason,
        order.CreatedAt,
        order.UpdatedAt);
}