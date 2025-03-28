// <copyright file="Filters.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Net.Http.Headers;

namespace SjaInNumbers.Server.Endpoints;

public static class Filters
{
    public static RouteHandlerBuilder WithNoCache(this RouteHandlerBuilder builder)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var result = await next(context);

            var header = new CacheControlHeaderValue
            {
                NoStore = true,
            };

            context.HttpContext.Response.GetTypedHeaders().CacheControl = header;

            return result;
        });

        return builder;
    }
}
