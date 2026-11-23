using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderCase.Contracts.Events;
using OrderCase.Processing.Application.Abstractions.Messaging;
using OrderCase.Processing.Application.Abstractions.Payments;
using OrderCase.Processing.Domain.ProcessedOrders;
using OrderCase.Processing.Persistence;

namespace OrderCase.Processing.Application.Features.ProcessedOrders.Commands.ProcessOrder;

public sealed class ProcessOrderCommandHandler(
    ProcessingDbContext dbContext,
    IPaymentGateway paymentGateway,
    IEventPublisher eventPublisher,
    ILogger<ProcessOrderCommandHandler> logger)
    : IRequestHandler<ProcessOrderCommand>
{
    public async Task Handle(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var alreadyProcessed = await dbContext.ProcessedOrders
            .AnyAsync(p => p.OrderId == request.OrderId && !p.IsDeleted, cancellationToken);

        if (alreadyProcessed)
        {
            logger.LogInformation("Order {OrderId} was already processed, duplicate message ignored", request.OrderId);
            return;
        }

        var paymentRequest = new PaymentRequest(
            request.OrderId,
            request.UserId,
            request.ProductId,
            request.Quantity,
            request.PaymentMethod);
        
        var paymentResult = await paymentGateway.ChargeAsync(paymentRequest, cancellationToken);

        if (paymentResult.IsSuccess)
            await HandleSuccessAsync(request, cancellationToken);
        else
            await HandleFailureAsync(request, paymentResult.FailureReason!, cancellationToken);
    }

    private async Task HandleSuccessAsync(ProcessOrderCommand request, CancellationToken cancellationToken)
    {
        var processedOrder = ProcessedOrder.Succeeded(
            request.OrderId, request.UserId, request.ProductId, request.Quantity, request.PaymentMethod);

        dbContext.ProcessedOrders.Add(processedOrder);

        await eventPublisher.PublishAsync(
            Topics.OrderCompleted,
            new OrderCompletedEvent(request.OrderId, DateTime.UtcNow),
            cancellationToken);

        logger.LogInformation("Payment succeeded for order {OrderId}", request.OrderId);
    }

    private async Task HandleFailureAsync(ProcessOrderCommand request, string reason, CancellationToken cancellationToken)
    {
        var processedOrder = ProcessedOrder.Failed(
            request.OrderId, request.UserId, request.ProductId, request.Quantity, request.PaymentMethod, reason);

        dbContext.ProcessedOrders.Add(processedOrder);

        await eventPublisher.PublishAsync(
            Topics.OrderFailed,
            new OrderFailedEvent(request.OrderId, reason, DateTime.UtcNow),
            cancellationToken);

        logger.LogWarning("Payment failed for order {OrderId}: {Reason}", request.OrderId, reason);
    }
}