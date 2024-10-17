// <copyright file="DailyReportCacheAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace SjaInNumbers.Server.Controllers.Filters;

/// <summary>
/// Attribute to indicate that this item can be cached but needs to be revalidated.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RevalidateCacheAttribute : ActionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        var header = new CacheControlHeaderValue
        {
            NoCache = true,
            MustRevalidate = true,
            Private = true,
        };

        context.HttpContext.Response.GetTypedHeaders().CacheControl = header;

        base.OnResultExecuting(context);
    }
}
