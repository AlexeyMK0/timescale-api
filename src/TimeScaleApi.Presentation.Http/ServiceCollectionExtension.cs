using TimeScaleApi.Presentation.Http.Settings;

namespace TimeScaleApi.Presentation.Http;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPresentationHttp(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(FileRecordController).Assembly);
        services.Configure<FileRecordControllerSettings>(configuration.GetSection(nameof(FileRecordControllerSettings)));
        return services;
    }
}