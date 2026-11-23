namespace OrderCase.Contracts.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    string UserId,
    string ProductId,
    int Quantity,
    string PaymentMethod,
    DateTime CreatedAt);