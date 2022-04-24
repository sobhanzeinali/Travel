using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Travel.Application;
using Travel.Data;
using Travel.Data.Postgres;
using Travel.Identity;
using Travel.Identity.Helpers;
using Travel.Shared;
using Travel.WebApi.Extensions;
using Travel.WebApi.Filters;
using Travel.WebApi.Helpers;

// Serilog
var name = Assembly.GetExecutingAssembly().GetName();
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
Log.Information("Starting host");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hbc, lc) =>
        lc.MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Assembly", $"{name.Name}")
            .Enrich.WithProperty("Assembly", $"{name.Version}")
            .WriteTo.Console()
            .ReadFrom.Configuration(hbc.Configuration)
    );
    // Add services to the container.
    builder.Services.AddApplication(builder.Configuration);
    builder.Services.AddInfrastructureData(builder.Configuration);
    builder.Services.AddInfrastructureShared(builder.Configuration);
    builder.Services.AddInfrastructureIdentity(builder.Configuration);
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddControllers();

    builder.Services.AddApiVersioningExtension();
    builder.Services.AddVersionedApiExplorerExtension();
    builder.Services.AddSwaggerGenExtension();


    builder.Services.AddControllersWithViews(options => options.Filters.Add(new ApiExceptionFilter()));
    builder.Services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = true);


    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        IApiVersionDescriptionProvider provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseDeveloperExceptionPage();
        app.UseSwaggerExtension(provider);

        new ServiceCollection()
            .AddLogging(config => config.AddConsole())
            .AddMigrationService(builder.Configuration)
            .BuildServiceProvider()
            .GetService<IDataSchemaMigrator>()?.UpdateSchema();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseMiddleware<JwtMiddleware>();
    app.UseAuthorization();
    app.UseEndpoints(endpoints => endpoints.MapControllers());

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}