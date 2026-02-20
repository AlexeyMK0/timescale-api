using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TimeScaleApi.Application.Contracts;
using TimeScaleApi.Application.Contracts.FileRecords;
using TimeScaleApi.Application.Contracts.FileRecords.Operations;
using TimeScaleApi.Presentation.Http.Settings;

namespace TimeScaleApi.Presentation.Http;

[Route("api/file")]
[ApiController]
public class FileRecordController : ControllerBase
{   
    private readonly IFileRecordService _fileRecordService;

    private readonly ILogger<FileRecordController> _logger;

    private readonly int _defaultNumberOfRecords;
    
    public FileRecordController(
        IFileRecordService fileRecordService,
        ILogger<FileRecordController> logger,
        IOptions<FileRecordControllerSettings> options)
    {
        const int numberOfRecordsByDefault = 10;
        
        _fileRecordService = fileRecordService;
        _logger = logger;
        _defaultNumberOfRecords = options?.Value?.DefaultNumberOfRecords ?? numberOfRecordsByDefault;
    }

    [HttpPost]
    public async Task<ActionResult<FileRecordDto>> ImportFile(
        [FromForm] ImportFileDto fileDto,
        CancellationToken cancellationToken)
    {
        IFormFile file = fileDto.File;
        _logger.LogInformation("Importing file {file}", file.FileName);

        string fileName = file.FileName;
        using var stream = file.OpenReadStream();
        var request = new ImportFile.Request(fileName, stream);

        ImportFile.Response response = await _fileRecordService.ImportFile(request, cancellationToken);
        return response switch
        {
            ImportFile.Response.Success success => Ok(success.NewRecord),
            ImportFile.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet]
    public async Task<ActionResult<FileRecordDto[]>> Get(
        [FromQuery] FileRecordQueryDto queryDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting file records");
        FindRecords.Request request = new FindRecords.Request(
            queryDto.FileName,
            queryDto.MinStartDateTime,
            queryDto.MaxStartDateTime,
            queryDto.MinAverageValue,
            queryDto.MaxAverageValue,
            queryDto.MinAverageExecutionTime,
            queryDto.MaxAverageExecutionTime);

        FindRecords.Response response = await _fileRecordService.FindRecords(request, cancellationToken);
        return response switch
        {
            FindRecords.Response.Success success => Ok(success.FoundRecords),
            FindRecords.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet]
    [Route("recent")]
    public async Task<ActionResult<FileRecordDto[]>> GetRecent(
        [FromQuery(Name = "n")] [Required] string name,
        [FromQuery(Name = "q")] [Range(1, 10000)]
        int? numberOfRecords,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Getting recent file records");
        var request = new GetRecent.Request(numberOfRecords ?? _defaultNumberOfRecords, name);

        GetRecent.Response response = await _fileRecordService.GetByNameSortedByStartDate(request, cancellationToken);
        return response switch
        {
            GetRecent.Response.Failure failure => BadRequest(failure.Message),
            GetRecent.Response.Success success => Ok(success.FoundData),
            _ => throw new UnreachableException(),
        };
    }
}