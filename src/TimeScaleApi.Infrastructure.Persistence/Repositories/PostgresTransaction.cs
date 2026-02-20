using Microsoft.EntityFrameworkCore.Storage;
using TimeScaleApi.Application.Abstractions.Repository;

namespace TimeScaleApi.Infrastructure.Persistence.Repositories;

public class PostgresTransaction : ITransaction
{
    private IDbContextTransaction _transaction;

    public PostgresTransaction(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public async ValueTask DisposeAsync() => await _transaction.DisposeAsync();

    public void Dispose() => _transaction.Dispose();

    public async Task CommitAsync(CancellationToken cancellationToken)
        => await _transaction.CommitAsync(cancellationToken);

    public void Commit() => _transaction.Commit();

    public async Task RollbackAsync(CancellationToken cancellationToken)
        => await _transaction.RollbackAsync(cancellationToken);

    public void Rollback() => _transaction.Rollback();
}