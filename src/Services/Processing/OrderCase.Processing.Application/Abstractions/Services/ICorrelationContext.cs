namespace OrderCase.Processing.Application.Abstractions.Services;

public interface ICorrelationContext
{
    string CorrelationId { get; }
    void Set(string correlationId);
}