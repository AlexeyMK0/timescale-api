namespace TimeScaleApi.Application.Contracts.FileRecords.Operations;

public class FindRecords
{
    public sealed record Request(
        string? File,
        DateTimeOffset? MinStartDateTime,
        DateTimeOffset? MaxStartDateTime,
        double? MinAverageValue,
        double? MaxAverageValue,
        double? MinAverageExecutionTime,
        double? MaxAverageExecutionTime);

    public abstract record Response
    {
        public sealed record Success(FileRecordDto[] FoundRecords) : Response;
        
        public sealed record Failure(string Message) : Response;
    }
}