using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeScaleApi.Application.Contracts.FileRecords;
using TimeScaleApi.Application.ImportEntities;
using TimeScaleApi.Application.Options;
using TimeScaleApi.Application.Services;

namespace TimeScaleApi.Application;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMedianCalculator, MedianCalculator>();
        services.AddTransient<IFileRecordBuilder, FileRecordBuilder>();
        services.AddScoped<IFileRecordService, FileRecordService>();
        
        services.Configure<ServiceSettings>(configuration.GetSection("FileRecordServiceSettings"));
        
        return services;
    }
}