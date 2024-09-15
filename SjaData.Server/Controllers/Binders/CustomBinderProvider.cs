// <copyright file="CustomBinderProvider.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.ModelBinding;
using SjaData.Server.Model;

namespace SjaData.Server.Controllers.Binders;

/// <summary>
/// Provider that provides the custom model binders.
/// </summary>
public class CustomBinderProvider : IModelBinderProvider
{
    /// <inheritdoc/>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(Region))
        {
            return new RegionBinder();
        }
        else if (context.Metadata.ModelType == typeof(Trust))
        {
            return new TrustBinder();
        }

        return null;
    }
}
