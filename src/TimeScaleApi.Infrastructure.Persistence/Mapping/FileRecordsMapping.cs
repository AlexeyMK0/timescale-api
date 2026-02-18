using TimeScaleApi.Domain;
using TimeScaleApi.Infrastructure.Persistence.Entities;

namespace TimeScaleApi.Infrastructure.Persistence.Mapping;

public static class FileRecordsMapping
{
    public static FileRecord ToDomain(this FileRecordEntity record)
    {
        return new FileRecord(
            record.Name,
            record.DeltaTime,
            record.MinStartDateTime,
            record.AverageExecutionTime,
            record.AverageValue,
            record.MedianValue,
            record.MaxValue,
            record.MinValue);
    }

    public static FileRecordEntity ToEntity(this FileRecord domain)
    {
        return new FileRecordEntity
        {
            Name = domain.Name,
            AverageExecutionTime = domain.AverageExecutionTime,
            AverageValue = domain.AverageValue,
            DeltaTime = domain.DeltaTime,
            MaxValue = domain.MaxValue,
            MedianValue = domain.MedianValue,
            MinStartDateTime = domain.MinStartDateTime.ToUniversalTime(),
            MinValue = domain.MinValue
        };
    }
}