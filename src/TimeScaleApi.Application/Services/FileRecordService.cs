using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TimeScaleApi.Application.Abstractions.Parser;
using TimeScaleApi.Application.Abstractions.Repository;
using TimeScaleApi.Application.Abstractions.Repository.Queries;
using TimeScaleApi.Application.Contracts.FileRecords;
using TimeScaleApi.Application.Contracts.FileRecords.Operations;
using TimeScaleApi.Application.ImportEntities;
using TimeScaleApi.Application.Mapping;
using TimeScaleApi.Application.Options;
using TimeScaleApi.Domain;

namespace TimeScaleApi.Application.Services;

public class FileRecordService : IFileRecordService
{
    private readonly DateTimeOffset _minDateTime;
    private readonly int _maxDataRecordCount;
    private readonly int _minDataRecordCount;

    private readonly ICsvParser _csvParser;

    private readonly IPersistenceContext _persistenceContext;

    private readonly IMedianCalculator _medianCalculator;

    private readonly ILogger<FileRecordService> _logger;

    public FileRecordService(
        ICsvParser csvParser,
        IPersistenceContext persistenceContext,
        IMedianCalculator medianCalculator,
        ILogger<FileRecordService> logger,
        IOptions<ServiceSettings> serviceSettings)
    {
        const int defaultMaxDataRecordCount = 10000; 
        const int defaultMinDataRecordCount = 1;
        DateTimeOffset defaultMinDateTime
            = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        _csvParser = csvParser;
        _persistenceContext = persistenceContext;
        _medianCalculator = medianCalculator;
        _logger = logger;
        _maxDataRecordCount = serviceSettings?.Value?.MaxDataRecordCount ?? defaultMaxDataRecordCount;
        _minDataRecordCount = serviceSettings?.Value?.MinDataRecordCount ?? defaultMinDataRecordCount;
        _minDateTime = defaultMinDateTime;
    }

    public async Task<ImportFile.Response> ImportFile(ImportFile.Request request, CancellationToken cancellationToken)
    {
        ParseResult parseResult = _csvParser.ParseFile(request.Stream, request.FileName);

        if (parseResult is ParseResult.Failure parseFailure)
        {
            _logger.LogError("Error while parsing {}", parseFailure.Message);
            return new ImportFile.Response.Failure(parseFailure.Message);
        }

        List<DataRecord> dataRecords = ((ParseResult.Success)parseResult).records.ToList();

        var builder =
            new FileRecordBuilder(_minDateTime, _maxDataRecordCount, _minDataRecordCount, _medianCalculator);
        builder.WithName(request.FileName);

        // TODO: add transaction entity 
        await _persistenceContext.BeginTransactionAsync(cancellationToken);

        ArgumentNullException.ThrowIfNull(_persistenceContext.DataRecordsRepository);
        ArgumentNullException.ThrowIfNull(_persistenceContext.FileRecordsRepository);

        try
        {
            _logger.LogInformation("Starting file record import");
            if (_persistenceContext.DataRecordsRepository is null)
                throw new InvalidOperationException("DataRecordsRepository is null");
            await _persistenceContext.DataRecordsRepository.RemoveAllByFileNameAsync(request.FileName,
                cancellationToken);
            _logger.LogInformation("Successfully removed data records by {}", request.FileName);

            if (_persistenceContext.FileRecordsRepository is null)
                throw new InvalidOperationException("FileRecordsRepository is null");
            await _persistenceContext.FileRecordsRepository.RemoveAllByFileNameAsync(request.FileName,
                cancellationToken);
            _logger.LogInformation("Successfully removed file records by {}", request.FileName);

            foreach (DataRecord dataRecord in dataRecords)
            {
                string? error = builder.AddDataRecord(dataRecord);
                if (error is not null)
                {
                    await _persistenceContext.RollbackTransactionAsync(cancellationToken);
                    return new ImportFile.Response.Failure(error);
                }
            }

            IFileRecordBuilder.BuildResult buildResult = builder.Build();
            if (buildResult is IFileRecordBuilder.BuildResult.Failure buildFailure)
            {
                await _persistenceContext.RollbackTransactionAsync(cancellationToken);
                return new ImportFile.Response.Failure(buildFailure.Message);
            }

            FileRecord result = ((IFileRecordBuilder.BuildResult.Success)buildResult).Record;
            _persistenceContext.DataRecordsRepository.SaveRange(dataRecords);
            _persistenceContext.FileRecordsRepository.Save(result);

            await _persistenceContext.SaveChangesAsync(cancellationToken);
            await _persistenceContext.CommitTransactionAsync(cancellationToken);
            return new ImportFile.Response.Success(result.MapToDto());
        }
        catch (Exception e)
        {
            _logger.LogError("Unknown error: {}", e.Message);
            await _persistenceContext.RollbackTransactionAsync(cancellationToken);
            return new ImportFile.Response.Failure(e.Message);
        }
    }

    public async Task<FindRecords.Response> FindRecords(FindRecords.Request request, CancellationToken cancellationToken)
    {
        var query = new FileRecordQuery(
            request.File,
            request.MinStartDateTime,
            request.MaxStartDateTime,
            request.MinAverageValue,
            request.MaxAverageValue,
            request.MinAverageExecutionTime,
            request.MaxAverageExecutionTime);

        var result = await _persistenceContext.FileRecordsRepository.GetAsync(query, cancellationToken);
        
        return new FindRecords.Response.Success(result
            .Select(domain => domain.MapToDto())
            .ToArray());
    }

    public async Task<GetRecent.Response> GetByNameSortedByStartDate(GetRecent.Request request, CancellationToken cancellationToken)
    {
        var query = new DataRecordQueryLatest(request.Name, request.Quantity);
        var result = await _persistenceContext.DataRecordsRepository.GetAsync(query, cancellationToken);
        return new GetRecent.Response.Success(
                result
                .Select(domain => domain.MapToDto())
                .ToArray());
    }
}