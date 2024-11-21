// <copyright file="ConfigureSwaggerOptions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SjaInNumbers.Server.Helpers;

/// <summary>
/// Options to configure swagger generation.
/// </summary>
/// <param name="provider">API Description Provider service.</param>
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
                    Title = "SJA In Numbers API",
                    Description = "API to provide data for the SJA In Numbers application.",
                    TermsOfService = new Uri("https://dashboard.tr-toolbox.me.uk/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Tony Richards",
                        Email = "tony.richards@sja.org.uk",
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    },
                    Version = $"v{description.ApiVersion}",
                });
        }
    }
}
