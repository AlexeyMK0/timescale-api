using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TimeScaleApi.Infrastructure.Persistence.Entities;

[Table("Results")]
public sealed class FileRecordEntity
{   
    [Key]
    public string Name { get; set; }
    
    public TimeSpan DeltaTime { get; set; }
    
    public DateTimeOffset MinStartDateTime { get; set; }
    
    public double AverageExecutionTime { get; set; }
    
    public double AverageValue { get; set; }
    
    public double MedianValue { get; set; }
    
    public double MaxValue { get; set; }
    
    public double MinValue { get; set; }
}