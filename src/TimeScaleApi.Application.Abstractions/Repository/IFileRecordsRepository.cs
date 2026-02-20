using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Abstractions.Repository;

public interface IFileRecordsRepository
{
    FileRecord Save(FileRecord record);

    Task<FileRecord[]> GetAsync(FileRecordQuery query, CancellationToken cancellationToken);
    
    Task RemoveAllByFileNameAsync(string fileName, CancellationToken cancellationToken);
}