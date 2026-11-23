using MediatR;
using Microsoft.Extensions.Logging;
using OrderCase.Contracts.Events;
using OrderCase.Ordering.Application.Abstractions.Messaging;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Application.Common.Exceptions;
using OrderCase.Ordering.Domain.Orders;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandHandler(
    OrderingDbContext dbContext,
    IEventPublisher eventPublisher,
    ICurrentUserService currentUser,
    ILogger<CreateOrderCommandHandler> logger)
    : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAdmin && request.UserId != currentUser.UserId)
            throw new ForbiddenException("Users can only create orders for themselves.");

        var order = Order.Create(request.UserId, request.ProductId, request.Quantity, request.PaymentMethod);

        dbContext.Orders.Add(order);

        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            order.UserId,
            order.ProductId,
            order.Quantity,
            order.PaymentMethod.ToString(),
            DateTime.UtcNow);

        await eventPublisher.PublishAsync(Topics.OrderCreated, orderCreatedEvent, cancellationToken);

        logger.LogInformation("Order {OrderId} created for user {UserId}", order.Id, order.UserId);

        return new CreateOrderResult(order.Id, order.Status.ToString());
    }
}