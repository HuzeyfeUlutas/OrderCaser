using MediatR;
using OrderCase.Ordering.Application.Abstractions.Persistence;
using OrderCase.Ordering.Application.Common.Requests;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    OrderingDbContext dbContext,
    ITransactionManager transactionManager)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITransactionalRequest)
            return await next(cancellationToken);

        await transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next(cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transactionManager.CommitAsync(cancellationToken);
            return response;
        }
        catch
        {
            await transactionManager.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}