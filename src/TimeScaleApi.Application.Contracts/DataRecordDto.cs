namespace TimeScaleApi.Application.Contracts;

public record DataRecordDto(
    string FileName,
    DateTimeOffset Date,
    long ExecutionTime,
    double Value);