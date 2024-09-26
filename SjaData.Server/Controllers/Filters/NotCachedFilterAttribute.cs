// <copyright file="NotCachedFilterAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace SjaData.Server.Controllers.Filters;

/// <summary>
/// Attribute to indicate that the response from an action must not be cached.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class NotCachedFilterAttribute : ActionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var header = new CacheControlHeaderValue
        {
            NoStore = true,
        };

        context.HttpContext.Response.GetTypedHeaders().CacheControl = header;

        base.OnResultExecuting(context);
    }
}
