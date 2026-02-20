namespace TimeScaleApi.Application.Contracts.FileRecords.Operations;

public static class ImportFile
{
    public sealed record Request(string FileName, Stream Stream);

    public abstract record Response
    {
        public sealed record Success(FileRecordDto NewRecord) : Response;
        
        public sealed record Failure(string Message) : Response;
    }
}