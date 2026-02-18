using Microsoft.EntityFrameworkCore;
using TimeScaleApi.Infrastructure.Persistence.Entities;

namespace TimeScaleApi.Infrastructure.Persistence.DbContextModel;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<DataEntity> Values { get; set; }
    
    public DbSet<FileRecordEntity> Results { get; set; }
}