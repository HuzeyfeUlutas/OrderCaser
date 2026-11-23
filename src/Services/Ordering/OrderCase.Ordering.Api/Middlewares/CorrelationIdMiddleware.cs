using OrderCase.Contracts.Events;
using OrderCase.Ordering.Application.Abstractions.Services;
using Serilog.Context;

namespace OrderCase.Ordering.Api.Middlewares;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string LogPropertyName = "CorrelationId";

    public async Task InvokeAsync(HttpContext context, ICorrelationContext correlationContext)
    {
        var correlationId = context.Request.Headers.TryGetValue(MessageHeaders.CorrelationId, out var values)
                            && !string.IsNullOrWhiteSpace(values)
            ? values.ToString()
            : Guid.NewGuid().ToString();

        correlationContext.Set(correlationId);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[MessageHeaders.CorrelationId] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty(LogPropertyName, correlationId))
        {
            await next(context);
        }
    }
}