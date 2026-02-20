namespace TimeScaleApi.Application.Contracts;

public record FileRecordDto(
    string Name,
    TimeSpan DeltaTime,
    DateTimeOffset MinStartDateTime,
    double AverageExecutionTime,
    double AverageValue,
    double MedianValue,
    double MaxValue,
    double MinValue);