// <copyright file="HoursApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model.Hours;
using SjaData.Server.Api.Model;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Api;

public static class HoursApiExtensions
{
    public static WebApplication MapHoursApi(this WebApplication webApp)
    {
        var group = webApp.MapGroup("/api/hours");

        group.MapPost(string.Empty, AddHours)
            .Produces(StatusCodes.Status204NoContent);
        group.MapGet("target", GetHoursTarget)
            .Produces<HoursTarget>();
        group.MapGet("count", GetHoursCount)
            .Produces<HoursCount>()
            .Produces(StatusCodes.Status304NotModified);
        group.MapDelete("{id}", DeleteHours)
            .Produces(StatusCodes.Status204NoContent);

        return webApp;
    }

    internal static async Task<IResult> AddHours(NewHoursEntry hours, IHoursService hoursService, HttpContext context)
    {
        await hoursService.AddAsync(hours);
        return Results.NoContent();
    }

    internal static IResult GetHoursTarget()
    {
        return TypedResults.Ok(new HoursTarget { Target = 4000 });
    }

    internal static async Task<IResult> GetHoursCount(HoursQuery query, IHoursService hoursService, HttpContext context)
    {
        if (context.Request.GetTypedHeaders().IfModifiedSince.HasValue)
        {
            var age = await hoursService.GetLastModifiedAsync() - context.Request.GetTypedHeaders().IfModifiedSince;

            if (age < TimeSpan.FromSeconds(1))
            {
                return Results.StatusCode(StatusCodes.Status304NotModified);
            }
        }

        var count = await hoursService.CountAsync(query);

        context.Response.GetTypedHeaders().LastModified = count.LastUpdate;
        context.Response.GetTypedHeaders().CacheControl = new() { Private = true, NoCache = true };

        return Results.Ok(count);
    }

    internal static async Task<IResult> DeleteHours(int id, IHoursService hoursService)
    {
        await hoursService.DeleteAsync(id);
        return Results.NoContent();
    }
}
