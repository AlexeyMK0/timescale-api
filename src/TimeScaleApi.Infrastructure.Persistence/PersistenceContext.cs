using Microsoft.EntityFrameworkCore.Storage;
using TimeScaleApi.Application.Abstractions.Repository;
using TimeScaleApi.Infrastructure.Persistence.DbContextModel;
using TimeScaleApi.Infrastructure.Persistence.Repositories;

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

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return new PostgresTransaction(transaction);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}