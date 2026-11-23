namespace OrderCase.Contracts.Events;

public sealed record OrderFailedEvent(
    Guid OrderId,
    string Reason,
    DateTime FailedAt);