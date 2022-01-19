using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Travel.WebApi.Helpers;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Travel Tour",
            Version = description.ApiVersion.ToString(),
            Description = "web service for Travel Tour.",
            Contact = new OpenApiContact()
            {
                Email = "developer@traveltour.xyz",
                Name = "IT Department",
                Url = new Uri("https://traveltour.xyz/support")
            }
        };
        if (description.IsDeprecated)
        {
            info.Description += "<strong>This Api version of Travel Tour has been deprecated.</strong>";
        }

        return info;
    }
}