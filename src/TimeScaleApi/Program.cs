using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeScaleApi.Application;
using TimeScaleApi.Infrastructure.Parsing;
using TimeScaleApi.Infrastructure.Persistence;
using TimeScaleApi.Infrastructure.Persistence.DbContextModel;
using TimeScaleApi.Presentation.Http;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructureParsing()
    .AddInfrastructurePersistence()
    .AddDbContext<AppDbContext>(options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddPresentationHttp();

Console.Out.WriteLine("Default connection string: " + builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

// using var scope = app.Services.CreateScope();
// var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
// db.Database.EnsureCreated();

app.Run();