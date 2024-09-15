// <copyright file="ConfigureSwaggerOptions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SjaData.Server.Controllers.Filters;

/// <summary>
/// Sets swagger options for the versioned API.
/// </summary>
public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider = provider;

    /// <inheritdoc/>
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
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    },
                });
        }
    }
}
