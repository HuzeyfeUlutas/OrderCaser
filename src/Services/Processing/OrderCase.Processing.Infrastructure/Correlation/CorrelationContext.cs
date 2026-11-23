using OrderCase.Processing.Application.Abstractions.Services;

namespace OrderCase.Processing.Infrastructure.Correlation;

public sealed class CorrelationContext : ICorrelationContext
{
    private static readonly AsyncLocal<string?> Current = new();

    public string CorrelationId => Current.Value ??= Guid.NewGuid().ToString();

    public void Set(string correlationId) => Current.Value = correlationId;
}