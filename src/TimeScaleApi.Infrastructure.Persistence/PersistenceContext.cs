using TimeScaleApi.Application.Abstractions.Repository;
using TimeScaleApi.Infrastructure.Persistence.DbContextModel;

namespace TimeScaleApi.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    private readonly AppDbContext _context;

    public IFileRecordsRepository FileRecordsRepository { get; }
    public IDataRecordsRepository DataRecordsRepository { get; }

    public PersistenceContext(
        IFileRecordsRepository fileRecordsRepository,
        IDataRecordsRepository dataRecordsRepository,
        AppDbContext context)
    {
        FileRecordsRepository = fileRecordsRepository;
        DataRecordsRepository = dataRecordsRepository;
        _context = context;
    }

    public void BeginTransaction()
    {
        _context.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        _context.Database.CommitTransaction();
    }

    public void RollbackTransaction()
    {
        _context.Database.RollbackTransaction();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}