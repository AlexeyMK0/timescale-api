using Microsoft.Extensions.Logging;
using TimeScaleApi.Application.Abstractions.Parser;
using TimeScaleApi.Application.Abstractions.Repository;
using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Application.Contracts.FileRecords;
using TimeScaleApi.Application.Contracts.FileRecords.Operations;
using TimeScaleApi.Application.ImportEntities;
using TimeScaleApi.Application.Mapping;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Services;

public class FileRecordService : IFileRecordService
{
    private static readonly DateTimeOffset MinDateTime
        = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const int MaxDataRecordCount = 10000;
    private const int MinDataRecordCount = 1;

    private readonly ICsvParser _csvParser;

    private readonly IPersistenceContext _persistenceContext;

    private readonly IMedianCalculator _medianCalculator;

    private readonly ILogger<FileRecordService> _logger;
    
    public FileRecordService(ICsvParser csvParser, IPersistenceContext persistenceContext, IMedianCalculator medianCalculator, ILogger<FileRecordService> logger)
    {
        _csvParser = csvParser;
        _persistenceContext = persistenceContext;
        _medianCalculator = medianCalculator;
        _logger = logger;
    }

    public ImportFile.Response ImportFile(ImportFile.Request request)
    {
        ParseResult parseResult = _csvParser.ParseFile(request.Stream, request.FileName);

        if (parseResult is ParseResult.Failure parseFailure)
        {
            _logger.LogError("Error while parsing {}", parseFailure.Message);
            return new ImportFile.Response.Failure(parseFailure.Message);
        }

        List<DataRecord> dataRecords = ((ParseResult.Success)parseResult).records.ToList();

        var builder =
            new FileRecordBuilder(MinDateTime, MaxDataRecordCount, MinDataRecordCount, _medianCalculator);
        builder.WithName(request.FileName);

        // TODO: add transaction entity
        _persistenceContext.BeginTransaction();

        ArgumentNullException.ThrowIfNull(_persistenceContext.DataRecordsRepository);
        ArgumentNullException.ThrowIfNull(_persistenceContext.FileRecordsRepository);
        
        try
        {
            _logger.LogInformation("Starting file record import");
            if (_persistenceContext.DataRecordsRepository is null) throw new InvalidOperationException("DataRecordsRepository is null");
            _persistenceContext.DataRecordsRepository.RemoveAllByFileName(request.FileName);
            _logger.LogInformation("Successfully removed data records by {}", request.FileName);
            if (_persistenceContext.FileRecordsRepository is null) throw new InvalidOperationException("FileRecordsRepository is null");
            _persistenceContext.FileRecordsRepository.RemoveAllByFileName(request.FileName);
            _logger.LogInformation("Successfully removed file records by {}", request.FileName);

            foreach (DataRecord dataRecord in dataRecords)
            {
                string? error = builder.AddDataRecord(dataRecord);
                if (error is not null)
                {
                    _logger.LogError("Error while adding data record: {}", error);
                    _persistenceContext.RollbackTransaction();
                    return new ImportFile.Response.Failure(error);
                }

                _logger.LogInformation("Adding data record: {}", dataRecord);
                
                _persistenceContext.DataRecordsRepository.Save(dataRecord);
            }
            _logger.LogInformation("Successfully imported all data from {}", request.FileName);
            
            IFileRecordBuilder.BuildResult buildResult = builder.Build();
            if (buildResult is IFileRecordBuilder.BuildResult.Failure buildFailure)
            {
                _persistenceContext.RollbackTransaction();
                _logger.LogError("Error while building file record: {}", buildFailure.Message);
                return new ImportFile.Response.Failure(buildFailure.Message);
            }
            _logger.LogInformation("Successfully built record");
            
            FileRecord result = ((IFileRecordBuilder.BuildResult.Success)buildResult).Record;
            _persistenceContext.FileRecordsRepository.Save(result);
            _logger.LogInformation("Successfully saved record");

            _persistenceContext.SaveChanges();
            _persistenceContext.CommitTransaction();
            return new ImportFile.Response.Success(result.MapToDto());
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error: {}", e.Message);
            _persistenceContext.RollbackTransaction();
            return new ImportFile.Response.Failure(e.Message);
        }
    }

    public FindRecords.Response FindRecords(FindRecords.Request request)
    {
        var query = new FileRecordQuery(
            request.File,
            request.MinStartDateTime,
            request.MaxStartDateTime,
            request.MinAverageValue,
            request.MaxAverageValue,
            request.MinAverageExecutionTime,
            request.MaxAverageExecutionTime);

        return new FindRecords.Response.Success(_persistenceContext.FileRecordsRepository
            .Get(query)
            .Select(domain => domain.MapToDto())
            .ToArray());
    }

    public GetRecent.Response GetByNameSortedByStartDate(GetRecent.Request request)
    {
        var query = new DataRecordQueryLatest(request.Name, request.Quantity);
        return new GetRecent.Response.Success(
            _persistenceContext.DataRecordsRepository
                .Get(query)
                .Select(domain => domain.MapToDto())
                .ToArray());
    }
}