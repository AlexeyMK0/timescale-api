using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.ImportEntities;

public interface IFileRecordBuilder
{
    string? AddDataRecord(DataRecord dataRecord);

    BuildResult Build();
    
    public abstract record BuildResult
    {
        public sealed record Success(FileRecord Record) : BuildResult;

        public sealed record Failure(string Message) : BuildResult;
    }
}