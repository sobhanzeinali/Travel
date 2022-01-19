using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Travel.Application;
using Travel.Data;
using Travel.Data.Postgres;
using Travel.Shared;
using Travel.WebApi.Filters;
using Travel.WebApi.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructureData(builder.Configuration);
builder.Services.AddInfrastructureShared(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddControllersWithViews(options => options.Filters.Add(new ApiExceptionFilter()));
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c => c.OperationFilter<SwaggerDefaultValues>());

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
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();