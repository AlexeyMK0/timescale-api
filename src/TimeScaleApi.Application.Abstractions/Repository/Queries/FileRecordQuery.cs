namespace TimeScaleApi.Application.Abstractions.Repository.Queries;

public record FileRecordQuery(
    string? FileName,
    DateTimeOffset? MinStartDateTime,
    DateTimeOffset? MaxStartDateTime,
    double? MinAverageValue,
    double? MaxAverageValue,
    double? MinAverageExecutionTime,
    double? MaxAverageExecutionTime
);