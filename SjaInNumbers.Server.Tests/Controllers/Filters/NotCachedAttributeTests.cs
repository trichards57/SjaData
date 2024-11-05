// <copyright file="NotCachedFilterAttributeTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using SjaInNumbers.Server.Controllers.Filters;

namespace SjaInNumbers.Server.Tests.Controllers.Filters;

public class NotCachedAttributeTests
{
    [Fact]
    public void OnResultExecuting_SetsNoStoreHeader()
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

        var filter = new NotCachedAttribute();

        filter.OnResultExecuting(context);

        context.HttpContext.Response.Headers.Should().ContainKey(HeaderNames.CacheControl)
            .WhoseValue.Should().ContainSingle("no-store");
    }
}
