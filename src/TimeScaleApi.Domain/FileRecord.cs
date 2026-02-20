namespace TimeScaleApi.Domain;

public record FileRecord(
    string Name,
    TimeSpan DeltaTime,
    DateTimeOffset MinStartDateTime,
    double AverageExecutionTime,
    double AverageValue,
    double MedianValue,
    double MaxValue,
    double MinValue);