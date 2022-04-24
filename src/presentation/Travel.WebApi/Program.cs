using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.PostgreSQL;
using Swashbuckle.AspNetCore.SwaggerGen;
using Travel.Application;
using Travel.Data;
using Travel.Data.Options;
using Travel.Data.Postgres;
using Travel.Identity;
using Travel.Identity.Helpers;
using Travel.Shared;
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
    builder.Services.AddApplication();
    builder.Services.AddInfrastructureData(builder.Configuration);
    builder.Services.AddInfrastructureShared(builder.Configuration);
    builder.Services.AddInfrastructureIdentity(builder.Configuration);
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddControllers();

    builder.Services.AddControllersWithViews(options => options.Filters.Add(new ApiExceptionFilter()));
    builder.Services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = true);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen(c =>
    {
        c.OperationFilter<SwaggerDefaultValues>();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });
    });

    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    builder.Services.AddApiVersioning(config =>
    {
        config.DefaultApiVersion = new ApiVersion(1, 0);
        config.AssumeDefaultVersionWhenUnspecified = true;
        config.ReportApiVersions = true;
    });

    builder.Services.AddVersionedApiExplorer(options =>
        options.GroupNameFormat = "'v'VVV");


    var app = builder.Build();

    app.UseSerilogRequestLogging();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        IApiVersionDescriptionProvider provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });

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