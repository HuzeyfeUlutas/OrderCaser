using MediatR;
using OrderCase.Processing.Application.Abstractions.Persistence;
using OrderCase.Processing.Application.Common.Requests;
using OrderCase.Processing.Persistence;

namespace OrderCase.Processing.Application.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(
    ProcessingDbContext dbContext,
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
            return await next();

        await transactionManager.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next();
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