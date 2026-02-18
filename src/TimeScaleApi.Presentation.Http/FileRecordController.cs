using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TimeScaleApi.Application.Contracts;
using TimeScaleApi.Application.Contracts.FileRecords;
using TimeScaleApi.Application.Contracts.FileRecords.Operations;

namespace TimeScaleApi.Presentation.Http;

[Route("api/file")]
[ApiController]
public class FileRecordController : ControllerBase
{
    private readonly IFileRecordService _fileRecordService;

    private readonly ILogger<FileRecordController> _logger;
    
    public FileRecordController(IFileRecordService fileRecordService, ILogger<FileRecordController> logger)
    {
        _fileRecordService = fileRecordService;
        _logger = logger;
    }

    [HttpPost]
    public ActionResult<FileRecordDto> ImportFile([FromForm] ImportFileDto fileDto)
    {
        IFormFile file = fileDto.File;
        _logger.LogInformation("Importing file {file}", file.FileName);
        
        string fileName = file.FileName;
        using var stream = file.OpenReadStream();
        var request = new ImportFile.Request(fileName, stream);

        ImportFile.Response response = _fileRecordService.ImportFile(request);
        return response switch
        {
            ImportFile.Response.Success success => Ok(success.NewRecord),
            ImportFile.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet]
    public ActionResult<FileRecordDto[]> Get([FromQuery] FileRecordQueryDto queryDto)
    {
        FindRecords.Request request = new FindRecords.Request(
            queryDto.FileName,
            queryDto.MinStartDateTime,
            queryDto.MaxStartDateTime,
            queryDto.MinAverageValue,
            queryDto.MaxAverageValue,
            queryDto.MinAverageExecutionTime,
            queryDto.MaxAverageExecutionTime);

        FindRecords.Response response = _fileRecordService.FindRecords(request);
        return response switch
        {
            FindRecords.Response.Success success => Ok(success.FoundRecords),
            FindRecords.Response.Failure failure => BadRequest(failure.Message),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet]
    [Route("recent")]
    public ActionResult<FileRecordDto[]> GetRecent(
        [FromQuery(Name = "n")] [Required] string name,
        [FromQuery(Name = "q")] [Range(1, 10000)]
        int numberOfRecords = 10
    )
    {
        var request = new GetRecent.Request(numberOfRecords, name);

        GetRecent.Response response = _fileRecordService.GetByNameSortedByStartDate(request);
        return response switch
        {
            GetRecent.Response.Failure failure => BadRequest(failure.Message),
            GetRecent.Response.Success success => Ok(success.FoundData),
            _ => throw new UnreachableException(),
        };
    }
}