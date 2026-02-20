using Microsoft.EntityFrameworkCore;
using TimeScaleApi.Infrastructure.Persistence.Entities;

namespace TimeScaleApi.Infrastructure.Persistence.DbContextModel;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }

    public DbSet<DataEntity> Values { get; set; }
    
    public DbSet<FileRecordEntity> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataEntity>()
            .HasIndex(dataEntity => dataEntity.Date);
        
        modelBuilder.Entity<DataEntity>()
            .HasIndex(dataEntity => dataEntity.FileName);
        
        modelBuilder.Entity<FileRecordEntity>()
            .HasIndex(dataEntity => dataEntity.Name);

        modelBuilder.Entity<FileRecordEntity>()
            .HasIndex(fileEntity => fileEntity.Name);
    }
}