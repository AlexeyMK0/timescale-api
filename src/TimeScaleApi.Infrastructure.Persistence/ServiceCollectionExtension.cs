using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeScaleApi.Application.Abstractions.Repository;
using TimeScaleApi.Infrastructure.Persistence.Repositories;

namespace TimeScaleApi.Infrastructure.Persistence;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services)
    {
        services.AddSingleton<IDataRecordsRepository, PostgresDataRepository>();
        services.AddSingleton<IFileRecordsRepository, PostgresFileRecordsRepository>();
        services.AddSingleton<IPersistenceContext, PersistenceContext>();
        return services;
    }
}