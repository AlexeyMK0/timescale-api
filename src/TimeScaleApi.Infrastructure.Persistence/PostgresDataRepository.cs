using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeScaleApi.Application.Abstractions.Repository;
using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Domain;
using TimeScaleApi.Infrastructure.Persistence.DbContextModel;
using TimeScaleApi.Infrastructure.Persistence.Mapping;

namespace TimeScaleApi.Infrastructure.Persistence;

public sealed class PostgresDataRepository : IDataRecordsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<PostgresDataRepository> _logger;

    public PostgresDataRepository(AppDbContext context, ILogger<PostgresDataRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public DataRecord Save(DataRecord record)
    {
        return _context.Add(record.ToEntity()).Entity.ToDomain();
    }

    public DataRecord[] Get(DataRecordQueryLatest queryLatest)
    {
        return _context.Values.AsNoTracking()
            .Where(d => d.FileName == queryLatest.FileName)
            .OrderByDescending(d => d.Date)
            .Take(queryLatest.LatestLimit)
            .Select(d => d.ToDomain())
            .ToArray();
    }

    public void RemoveAllByFileName(string fileName)
    {
        _context.Values
            .Where(d => d.FileName == fileName)
            .ExecuteDelete();
    }
}