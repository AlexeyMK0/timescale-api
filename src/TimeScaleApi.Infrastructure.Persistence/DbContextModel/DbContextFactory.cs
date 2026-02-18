using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TimeScaleApi.Infrastructure.Persistence.DbContextModel;

public class DbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(
            Environment.GetEnvironmentVariable("DATABASE_URL") ??
            "Host=localhost;Port=5432;Database=tsa_db;Username=postgres;Password=password"
        );
        
        return new AppDbContext(optionsBuilder.Options);
    }
}