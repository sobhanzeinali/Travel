using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travel.Application.Common.Interfaces;
using Travel.Data.Contexts;
using Travel.Data.Options;
using Travel.Data.Postgres;
using Travel.Domain.Settings;

namespace Travel.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureData(this IServiceCollection services, IConfiguration config)
    {
        var postgresOption = config.GetSection("db").Get<PostgresOptions>();
        services.Configure<PostgresOptions>(config.GetSection("db"));

        services.AddTransient<IDataSchemaMigrator, DataSchemaMigrator>();

        services.AddSingleton(postgresOption)
            .AddDbContext<ApplicationDbContext>(
                option => option.UseNpgsql(postgresOption.ConnectionString));
        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
        return services;
    }

    public static IServiceCollection AddMigrationService(this IServiceCollection services, IConfiguration config)
    {
        var postgresOption = config.GetSection("db").Get<PostgresOptions>();
        services.Configure<PostgresOptions>(config.GetSection("db"));
        return services
            .AddSingleton(postgresOption)
            .AddTransient<IDataSchemaMigrator, DataSchemaMigrator>();
    }
}