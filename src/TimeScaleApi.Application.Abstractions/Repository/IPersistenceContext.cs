namespace TimeScaleApi.Application.Abstractions.Repository;

public interface IPersistenceContext
{
    IFileRecordsRepository FileRecordsRepository { get; }
    IDataRecordsRepository DataRecordsRepository { get; }
    
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}