// <copyright file="EnumSchemaFilter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SjaData.Server.Model;

/// <summary>
/// Filter to make the swagger output work properly.
/// </summary>
public class EnumSchemaFilter : ISchemaFilter
{
    /// <inheritdoc/>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var val in Enum.GetValuesAsUnderlyingType(context.Type))
            {
                schema.Enum.Add(new OpenApiInteger((int)val));
            }
        }
    }
}
