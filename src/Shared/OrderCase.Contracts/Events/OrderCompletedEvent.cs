namespace OrderCase.Contracts.Events;

public sealed record OrderCompletedEvent(
    Guid OrderId,
    DateTime ProcessedAt);
    
    
    