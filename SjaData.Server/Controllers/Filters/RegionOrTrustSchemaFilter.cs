// <copyright file="RegionOrTrustSchemaFilter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.OpenApi.Models;
using SjaData.Model.Validation;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SjaData.Server.Controllers.Filters;

/// <summary>
/// Schema filter for the <see cref="RegionOrTrustAttribute"/>.
/// </summary>
public class RegionOrTrustSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.GetCustomAttributes(typeof(RegionOrTrustAttribute), true).FirstOrDefault() != default)
        {
            schema.Properties["region"].Description += " (Either this or trust must be set, but not both)";
            schema.Properties["trust"].Description += " (Either this or region must be set, but not both)";

            schema.OneOf =
            [
                new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        { "region", schema.Properties["region"] },
                    },
                },
                new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        { "trust", schema.Properties["trust"] },
                    },
                },
            ];

            schema.Properties.Remove("region");
            schema.Properties.Remove("trust");
        }
    }
}
