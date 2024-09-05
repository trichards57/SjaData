using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SjaData.Server.Controllers.Filters;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo()
                {
                    Title = "SJA AO Data API",
                    Description = "An API that provides data for AO reporting.",
                    Version = description.ApiVersion.ToString(),
                    Contact = new OpenApiContact()
                    {
                        Name = "Tony Richards",
                        Email = "tony.richards@sja.org.uk",
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url = new System.Uri("https://opensource.org/licenses/MIT"),
                    },
                });
        }
    }
}
