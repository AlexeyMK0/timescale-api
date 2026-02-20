using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Abstractions.Repository;

public interface IDataRecordsRepository
{   
    public void SaveRange(IEnumerable<DataRecord> records);
    
    Task<DataRecord[]> GetAsync(DataRecordQueryLatest queryLatest, CancellationToken cancellationToken);
    
    Task RemoveAllByFileNameAsync(string fileName, CancellationToken cancellationToken);
}