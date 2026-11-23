namespace OrderCase.Contracts.Events;

public sealed record OrderCompletedEvent(
    Guid OrderId,
    string UserId,
    string ProductId,
    int Quantity,
    string PaymentMethod,
    DateTime CreatedAt);
    
    
    