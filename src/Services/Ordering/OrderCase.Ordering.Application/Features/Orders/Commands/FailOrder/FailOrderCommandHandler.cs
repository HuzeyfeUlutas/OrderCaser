using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.FailOrder;

public sealed class FailOrderCommandHandler(
    OrderingDbContext dbContext,
    ILogger<FailOrderCommandHandler> logger)
    : IRequestHandler<FailOrderCommand>
{
    public async Task Handle(FailOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken);

        if (order is null)
        {
            logger.LogWarning("Order {OrderId} not found while failing, event ignored", request.OrderId);
            return;
        }

        order.Fail(request.Reason);

        logger.LogWarning("Order {OrderId} marked as failed: {Reason}", order.Id, request.Reason);
    }
}