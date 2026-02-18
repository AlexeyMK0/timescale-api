using TimeScaleApi.Application.Contracts;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Mapping;

public static class DataRecordMapper
{
    public static DataRecordDto MapToDto(this DataRecord record)
    {
        return new DataRecordDto(
            record.FileName,
            record.Date,
            record.ExecutionTime,
            record.Value);
    }
}