namespace OrderCase.Ordering.Application.Abstractions.Services;

public interface ICorrelationContext
{
    string CorrelationId { get; }
    void Set(string correlationId);
}