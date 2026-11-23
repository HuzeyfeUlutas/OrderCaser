using DotNetCore.CAP;
using MediatR;
using OrderCase.Contracts.Events;
using OrderCase.Processing.Application.Abstractions.Services;
using OrderCase.Processing.Application.Features.ProcessedOrders.Commands.ProcessOrder;
using OrderCase.Processing.Infrastructure;
using Serilog.Context;

namespace OrderCase.Processing.Worker.Subscribers;

public sealed class OrderCreatedSubscriber(
    ISender sender,
    ICorrelationContext correlationContext,
    ILogger<OrderCreatedSubscriber> logger) : ICapSubscribe
{
    [CapSubscribe(Topics.OrderCreated, Group = DependencyInjection.ConsumerGroup)]
    public async Task HandleAsync(
        OrderCreatedEvent @event,
        [FromCap] CapHeader headers,
        CancellationToken cancellationToken)
    {
        var correlationId = headers.TryGetValue(MessageHeaders.CorrelationId, out var value)
                            && !string.IsNullOrWhiteSpace(value)
            ? value
            : Guid.NewGuid().ToString();

        correlationContext.Set(correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            logger.LogInformation("Received {Topic} for order {OrderId}", Topics.OrderCreated, @event.OrderId);

            var command = new ProcessOrderCommand(
                @event.OrderId,
                @event.UserId,
                @event.ProductId,
                @event.Quantity,
                @event.PaymentMethod);

            await sender.Send(command, cancellationToken);
        }
    }
}