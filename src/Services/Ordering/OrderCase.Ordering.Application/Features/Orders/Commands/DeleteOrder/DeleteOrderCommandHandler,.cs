using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderCase.Ordering.Application.Abstractions.Services;
using OrderCase.Ordering.Application.Common.Exceptions;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Features.Orders.Commands.DeleteOrder;

public sealed class DeleteOrderCommandHandler(
    OrderingDbContext dbContext,
    ICurrentUserService currentUser,
    ILogger<DeleteOrderCommandHandler> logger)
    : IRequestHandler<DeleteOrderCommand>
{
    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAdmin)
            throw new ForbiddenException("Only admins can delete orders.");

        var order = await dbContext.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, cancellationToken)
                    ?? throw new NotFoundException($"Order '{request.OrderId}' was not found.");

        order.EnsureCanBeDeleted();

        dbContext.Orders.Remove(order);

        logger.LogInformation("Order {OrderId} deleted by {UserId}", order.Id, currentUser.UserId);
    }
}