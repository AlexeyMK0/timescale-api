namespace TimeScaleApi.Application.Abstractions.Repository;

public interface ITransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
    void Commit();
    Task RollbackAsync(CancellationToken cancellationToken);
    void Rollback();
}