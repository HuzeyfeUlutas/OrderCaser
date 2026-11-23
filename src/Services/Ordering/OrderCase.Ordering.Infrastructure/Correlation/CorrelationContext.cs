using OrderCase.Ordering.Application.Abstractions.Services;

namespace OrderCase.Ordering.Infrastructure.Correlation;

public class CorrelationContext : ICorrelationContext
{
    private static readonly AsyncLocal<string?> Current = new();

    public string CorrelationId => Current.Value ??= Guid.NewGuid().ToString();

    public void Set(string correlationId) => Current.Value = correlationId;
}