using TimeScaleApi.Application.Contracts.FileRecords.Operations;

namespace TimeScaleApi.Application.Contracts.FileRecords;

public interface IFileRecordService
{   
    ImportFile.Response ImportFile(ImportFile.Request request);
    
    FindRecords.Response FindRecords(FindRecords.Request request);
    
    GetRecent.Response GetByNameSortedByStartDate(GetRecent.Request request);
}