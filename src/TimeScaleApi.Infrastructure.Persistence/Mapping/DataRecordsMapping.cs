using TimeScaleApi.Domain;
using TimeScaleApi.Infrastructure.Persistence.Entities;

namespace TimeScaleApi.Infrastructure.Persistence.Mapping;

public static class DataRecordsMapping
{
    public static DataRecord ToDomain(this DataEntity entity)
    {
        return new DataRecord(
            entity.FileName,
            entity.Date,
            entity.ExecutionTime,
            entity.Value);
    }

    public static DataEntity ToEntity(this DataRecord domain)
    {
        return new DataEntity
        {
            FileName = domain.FileName,
            Date = domain.Date.ToUniversalTime(),
            ExecutionTime = domain.ExecutionTime,
            Value = domain.Value
        };
    }
}