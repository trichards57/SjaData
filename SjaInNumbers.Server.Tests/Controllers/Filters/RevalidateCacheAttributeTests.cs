// <copyright file="RevalidateCacheAttributeTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using SjaInNumbers.Server.Controllers.Filters;

namespace SjaInNumbers.Server.Tests.Controllers.Filters;

public class RevalidateCacheAttributeTests
{
    [Fact]
    public void OnActionExecuted_ShouldSetCacheControlHeader()
    {
        var context = new ResultExecutingContext(
                        new ActionContext()
                        {
                            HttpContext = new DefaultHttpContext(),
                            RouteData = new RouteData(),
                            ActionDescriptor = new ActionDescriptor(),
                        },
                        [],
                        new OkResult(),
                        new object());

        var attribute = new RevalidateCacheAttribute();

        attribute.OnResultExecuting(context);

        context.HttpContext.Response.Headers.Should().ContainKey(HeaderNames.CacheControl)
            .WhoseValue.Should().ContainSingle("no-cache, no-store, must-revalidate");
    }
}
