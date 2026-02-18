using Microsoft.AspNetCore.Mvc;

namespace TimeScaleApi.Presentation.Http;

public record FileRecordQueryDto(
    [FromQuery(Name = "fileName")]
    string? FileName,
    [FromQuery(Name = "minStartDateTime")]
    DateTime? MinStartDateTime,
    [FromQuery(Name = "maxStartDateTime")]
    DateTime? MaxStartDateTime,
    [FromQuery(Name = "minAverageValue")]
    double? MinAverageValue,
    [FromQuery(Name = "maxAverageValue")]
    double? MaxAverageValue,
    [FromQuery(Name = "minAverageExecutionTime")]
    double? MinAverageExecutionTime,
    [FromQuery(Name = "maxAverageExecutionTime")]
    double? MaxAverageExecutionTime
    );