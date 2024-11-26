// <copyright file="RevalidateCacheAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Server.Controllers.Filters;

/// <summary>
/// Attribute to indicate that this item can be cached but needs to be revalidated.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class RevalidateCacheAttribute : ActionFilterAttribute
{
    /// <inheritdoc/>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var header = new CacheControlHeaderValue
        {
            NoCache = true,
            MustRevalidate = true,
            Private = true,
        };

        context.HttpContext.Response.GetTypedHeaders().CacheControl = header;

        if (context.Result is ObjectResult result
            && result.StatusCode == StatusCodes.Status200OK
            && result.Value is IDateMarked item)
        {
            var etag = context.HttpContext.Request.Headers[HeaderNames.IfNoneMatch].FirstOrDefault();
            var actualEtag = new EntityTagHeaderValue($"\"{item.ETag}\"", true);
            var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);

            context.HttpContext.Response.GetTypedHeaders().ETag = actualEtag;
            context.HttpContext.Response.GetTypedHeaders().LastModified = item.LastModified;

            if (actualEtag.Compare(etagValue, false))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
            }
        }
    }
}
