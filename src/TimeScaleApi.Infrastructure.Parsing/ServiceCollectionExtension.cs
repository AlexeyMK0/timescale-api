
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeScaleApi.Application.Abstractions.Parser;

namespace TimeScaleApi.Infrastructure.Parsing;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructureParsing(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ParserSettings>(configuration.GetSection("ParserSettings"));
        services.AddTransient<ICsvParser, CsvParser>();
        return services;
    }
}