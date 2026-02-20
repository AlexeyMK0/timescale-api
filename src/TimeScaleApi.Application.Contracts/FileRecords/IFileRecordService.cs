using TimeScaleApi.Application.Contracts.FileRecords.Operations;

namespace TimeScaleApi.Application.Contracts.FileRecords;

public interface IFileRecordService
{   
    Task<ImportFile.Response> ImportFile(ImportFile.Request request, CancellationToken cancellationToken);
    
    Task<FindRecords.Response> FindRecords(FindRecords.Request request, CancellationToken cancellationToken);
    
    Task<GetRecent.Response> GetByNameSortedByStartDate(GetRecent.Request request, CancellationToken cancellationToken);
}