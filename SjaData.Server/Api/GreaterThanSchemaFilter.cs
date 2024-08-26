// <copyright file="GreaterThanSchemaFilter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.OpenApi.Models;
using SjaData.Model.Validation;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SjaData.Server.Api;

/// <summary>
/// Schema filter that applies the GreaterThanAttribute to the schema.
/// </summary>
public class GreaterThanSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var prop in context.Type.GetProperties())
        {
            if (prop.GetCustomAttributes(typeof(GreaterThanAttribute), true).FirstOrDefault() is GreaterThanAttribute attr)
            {
                var name = char.ToLowerInvariant(prop.Name[0]) + prop.Name[1..];

                schema.Properties[name].Minimum = attr.Minimum;
                schema.Properties[name].ExclusiveMinimum = true;
            }
        }
    }
}
