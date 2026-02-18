using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Abstractions.Repository;

public interface IDataRecordsRepository
{
    DataRecord Save(DataRecord record);
    
    DataRecord[] Get(DataRecordQueryLatest queryLatest);
    
    void RemoveAllByFileName(string fileName);
}