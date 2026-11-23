namespace OrderCase.Processing.Application.Abstractions.Payments;

public sealed record PaymentRequest(
    Guid OrderId,
    string UserId,
    string ProductId,
    int Quantity,
    string PaymentMethod);