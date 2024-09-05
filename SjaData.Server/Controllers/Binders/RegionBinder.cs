// <copyright file="RegionBinder.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using SjaData.Model;
using SjaData.Model.Converters;

namespace SjaData.Server.Controllers.Binders;

/// <summary>
/// Model binder for the Region enumeration.
/// </summary>
public class RegionBinder : IModelBinder
{
    /// <summary>
    /// Gets the OpenApiSchema matching this binder.
    /// </summary>
    public static OpenApiSchema Schema => RegionConverter.Schema;

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

        var res = RegionConverter.FromString(value ?? string.Empty);

        bindingContext.Result = res == Region.Undefined ? ModelBindingResult.Failed() : ModelBindingResult.Success(res);

        return Task.CompletedTask;
    }
}
