using TimeScaleApi.Application.Contracts;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Mapping;

public static class FileRecordMapper
{
    public static FileRecordDto MapToDto(this FileRecord fileRecord)
    {
        return new FileRecordDto(
            fileRecord.Name,
            fileRecord.DeltaTime,
            fileRecord.MinStartDateTime,
            fileRecord.AverageExecutionTime,
            fileRecord.AverageValue,
            fileRecord.MedianValue,
            fileRecord.MaxValue,
            fileRecord.MinValue);
    }   
}