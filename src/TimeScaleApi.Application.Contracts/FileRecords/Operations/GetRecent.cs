namespace TimeScaleApi.Application.Contracts.FileRecords.Operations;

public class GetRecent()
{
    public sealed record Request(int Quantity, string Name);

    public abstract record Response
    {
        public sealed record Success(DataRecordDto[] FoundData) : Response;
        
        public sealed record Failure(string Message) : Response;
    }
}