using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore.Storage;
using OrderCase.Ordering.Application.Abstractions.Persistence;
using OrderCase.Ordering.Persistence;

namespace OrderCase.Ordering.Infrastructure.Transactions;

public sealed class CapTransactionManager(
    OrderingDbContext dbContext,
    ICapPublisher capPublisher) : ITransactionManager
{
    private IDbContextTransaction? _transaction;
    
    public Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _transaction = dbContext.Database.BeginTransaction(capPublisher, autoCommit: false);
        return Task.CompletedTask;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _transaction.CommitAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await DisposeTransactionAsync();
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}