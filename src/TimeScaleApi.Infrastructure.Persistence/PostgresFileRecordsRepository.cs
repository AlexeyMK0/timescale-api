using Microsoft.EntityFrameworkCore;
using TimeScaleApi.Application.Abstractions.Repository;
using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Domain;
using TimeScaleApi.Infrastructure.Persistence.DbContextModel;
using TimeScaleApi.Infrastructure.Persistence.Entities;
using TimeScaleApi.Infrastructure.Persistence.Mapping;

namespace TimeScaleApi.Infrastructure.Persistence;

public class PostgresFileRecordsRepository : IFileRecordsRepository
{
    private readonly AppDbContext _context;

    public PostgresFileRecordsRepository(AppDbContext context)
    {
        _context = context;
    }

    public FileRecord Save(FileRecord record)
    {
        FileRecordEntity? foundRecord
            = _context.Results.Find(record.Name);
        if (foundRecord is null)
        {
            _context.Add(record.ToEntity());
            _context.SaveChanges();
            return record;
        }

        foundRecord.Name = record.Name;
        foundRecord.MinStartDateTime = record.MinStartDateTime;
        foundRecord.AverageExecutionTime = record.AverageExecutionTime;
        foundRecord.AverageValue = record.AverageValue;
        foundRecord.DeltaTime = record.DeltaTime;
        foundRecord.MaxValue = record.MaxValue;
        foundRecord.MinValue = record.MinValue;

        _context.SaveChanges();
        return foundRecord.ToDomain();
    }

    public async Task<FileRecord[]> GetAsync(FileRecordQuery query, CancellationToken cancellationToken)
    {
        return await _context.Results
            .AsNoTracking()
            .Where(r =>
                (query.FileName == null || r.Name == query.FileName)
                && (query.MaxAverageExecutionTime == null || r.AverageExecutionTime <= query.MaxAverageExecutionTime)
                && (query.MinAverageExecutionTime == null || r.AverageExecutionTime >= query.MinAverageExecutionTime)
                && (query.MinAverageValue == null || r.AverageValue >= query.MinAverageValue)
                && (query.MaxAverageValue == null || r.AverageValue <= query.MaxAverageValue)
                && (query.MinStartDateTime == null || r.MinStartDateTime >= query.MinStartDateTime)
                && (query.MaxStartDateTime == null || r.MinStartDateTime <= query.MaxStartDateTime)
            )
            .Select(r => r.ToDomain())
            .ToArrayAsync(cancellationToken);
    }

    public async Task RemoveAllByFileNameAsync(string fileName, CancellationToken cancellationToken)
    {
        await _context.Results
            .Where(r => r.Name == fileName)
            .ExecuteDeleteAsync(cancellationToken);
    }
}