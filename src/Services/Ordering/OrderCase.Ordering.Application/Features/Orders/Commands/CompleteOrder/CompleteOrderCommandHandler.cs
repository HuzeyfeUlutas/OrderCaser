using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.CompleteOrder;

public sealed class CompleteOrderCommandHandler(
    OrderingDbContext dbContext,
    ILogger<CompleteOrderCommandHandler> logger)
    : IRequestHandler<CompleteOrderCommand>
{
    public async Task Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found while completing, event ignored", request.OrderId);
            return;
        }

        order.Complete();

        logger.LogInformation("Order {OrderId} marked as completed", order.Id);
    }
}