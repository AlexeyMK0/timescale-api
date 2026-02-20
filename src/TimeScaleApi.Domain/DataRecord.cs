namespace TimeScaleApi.Domain;

public record DataRecord(
    string FileName,
    DateTimeOffset Date,
    long ExecutionTime,
    double Value);