using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeScaleApi.Infrastructure.Persistence.Entities;

[Table("Values")]
public class DataEntity
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string FileName { get; set; }
    
    public DateTimeOffset Date { get; set; }
    
    public long ExecutionTime { get; set; }

    public double Value { get; set; }
}