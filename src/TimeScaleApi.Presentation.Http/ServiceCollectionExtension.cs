namespace TimeScaleApi.Presentation.Http;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPresentationHttp(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(typeof(FileRecordController).Assembly);
        return services;
    }
}