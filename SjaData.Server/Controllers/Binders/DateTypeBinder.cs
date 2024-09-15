// <copyright file="DateTypeBinder.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SjaData.Server.Model;

namespace SjaData.Server.Controllers.Binders;

/// <summary>
/// Binder for <see cref="DateType"/> parameters.
/// </summary>
public class DateTypeBinder : IModelBinder
{
    /// <summary>
    /// Gets the OpenAPI schema for <see cref="DateType"/>.
    /// </summary>
    public static OpenApiSchema Schema =>
        new()
        {
            Title = "DateType",
            Type = "string",
            Enum = [
                new OpenApiString("d"),
                new OpenApiString("m"),
                new OpenApiString("y"),
            ],
        };

    /// <inheritdoc/>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        bindingContext.Result = value switch
        {
            "d" => ModelBindingResult.Success(DateType.Day),
            "m" => ModelBindingResult.Success(DateType.Month),
            "y" => ModelBindingResult.Success(DateType.Year),
            _ => ModelBindingResult.Failed(),
        };

        return Task.CompletedTask;
    }
}
