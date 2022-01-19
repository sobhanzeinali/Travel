using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travel.Application.Common.Interfaces;
using Travel.Domain.Settings;
using Travel.Shared.Files;
using Travel.Shared.Services;

namespace Travel.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureShared(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MailSettings>(config.GetSection("MailSettings"));
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();
        services.AddTransient<IDateTime, DateTimeServices>();
        services.AddTransient<IEmailService, EmailServices>();

        return services;
    }
}