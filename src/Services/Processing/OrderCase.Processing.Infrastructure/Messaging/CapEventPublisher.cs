using DotNetCore.CAP;
using OrderCase.Contracts.Events;
using OrderCase.Processing.Application.Abstractions.Messaging;
using OrderCase.Processing.Application.Abstractions.Services;

namespace OrderCase.Processing.Infrastructure.Messaging;

public sealed class CapEventPublisher(
    ICapPublisher capPublisher,
    ICorrelationContext correlationContext) : IEventPublisher
{
    public Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken)
        where TEvent : class
    {
        var headers = new Dictionary<string, string?>
        {
            [MessageHeaders.CorrelationId] = correlationContext.CorrelationId
        };

        return capPublisher.PublishAsync(topic, @event, headers, cancellationToken);
    }
}