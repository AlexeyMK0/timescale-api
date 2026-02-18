namespace TimeScaleApi.Application.Abstractions.Repository;

public interface IPersistenceContext
{
    IFileRecordsRepository FileRecordsRepository { get; }
    IDataRecordsRepository DataRecordsRepository { get; }
    
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();

    int SaveChanges();
}