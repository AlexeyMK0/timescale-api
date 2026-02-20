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

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}