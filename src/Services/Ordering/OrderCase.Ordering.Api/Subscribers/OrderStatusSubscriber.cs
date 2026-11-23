using DotNetCore.CAP;
using MediatR;
using OrderCase.Contracts.Events;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Application.Features.Orders.Commands.CompleteOrder;
using OrderCase.Ordering.Application.Features.Orders.Commands.FailOrder;
using OrderCase.Ordering.Infrastructure.Messaging;
using Serilog.Context;

namespace OrderCase.Ordering.Api.Subscribers;

public sealed class OrderStatusSubscriber(
    ISender sender,
    ICorrelationContext correlationContext,
    ILogger<OrderStatusSubscriber> logger) : ICapSubscribe
{
    [CapSubscribe(Topics.OrderCompleted, Group = ConsumerGroups.Ordering)]
    public Task HandleCompletedAsync(
        OrderCompletedEvent @event,
        [FromCap] CapHeader headers,
        CancellationToken cancellationToken) =>
        ExecuteWithCorrelationAsync(headers, Topics.OrderCompleted, @event.OrderId, () =>
            sender.Send(new CompleteOrderCommand(@event.OrderId), cancellationToken));

    [CapSubscribe(Topics.OrderFailed, Group = ConsumerGroups.Ordering)]
    public Task HandleFailedAsync(
        OrderFailedEvent @event,
        [FromCap] CapHeader headers,
        CancellationToken cancellationToken) =>
        ExecuteWithCorrelationAsync(headers, Topics.OrderFailed, @event.OrderId, () =>
            sender.Send(new FailOrderCommand(@event.OrderId, @event.Reason), cancellationToken));

    private async Task ExecuteWithCorrelationAsync(
        CapHeader headers,
        string topic,
        Guid orderId,
        Func<Task> action)
    {
        var correlationId = headers.TryGetValue(MessageHeaders.CorrelationId, out var value)
                            && !string.IsNullOrWhiteSpace(value)
            ? value
            : Guid.NewGuid().ToString();

        correlationContext.Set(correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            logger.LogInformation("Received {Topic} for order {OrderId}", topic, orderId);
            await action();
        }
    }
}