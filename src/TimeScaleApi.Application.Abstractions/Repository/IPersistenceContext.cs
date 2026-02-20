namespace TimeScaleApi.Application.Abstractions.Repository;

public interface IPersistenceContext
{
    IFileRecordsRepository FileRecordsRepository { get; }
    IDataRecordsRepository DataRecordsRepository { get; }
    
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}