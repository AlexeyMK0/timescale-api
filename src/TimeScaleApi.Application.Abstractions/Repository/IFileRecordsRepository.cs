using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Abstractions.Repository;

public interface IFileRecordsRepository
{
    FileRecord Save(FileRecord record);

    FileRecord[] Get(FileRecordQuery query);
    
    void RemoveAllByFileName(string fileName);
}