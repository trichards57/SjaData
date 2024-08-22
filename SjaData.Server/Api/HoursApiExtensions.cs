// <copyright file="HoursApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Identity.Web;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SjaData.Model.Hours;
using SjaData.Server.Api.Model;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Api;

/// <summary>
/// API extensions that handle hours records.
/// </summary>
public static partial class HoursApiExtensions
{
    /// <summary>
    /// Maps the hours API to the given web application.
    /// </summary>
    /// <param name="webApp">The web application to add to.</param>
    /// <returns><paramref name="webApp"/>, to allow method chaining.</returns>
    public static WebApplication MapHoursApi(this WebApplication webApp)
    {
        var group = webApp.MapGroup("/api/hours").WithOpenApi().WithTags("Hours");

        group.MapPost(string.Empty, AddHours)
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi(o =>
            {
                o.Responses[StatusCodes.Status204NoContent.ToString()].Description = "The hours entry was created successfully.";
                o.Summary = "Accepts a new hours record.";
                o.Description = "Accepts a new hours record.  If the provided person already has a shift logged on that day, it will be updated with the given information.  Only one entry per person per day is allowed.";
                return o;
            });
        group.MapGet("target", GetHoursTarget)
            .Produces<HoursTarget>()
            .WithOpenApi(o =>
            {
                o.Responses[StatusCodes.Status200OK.ToString()].Description = "The current NHS England hours target, represented as person-hours.";
                o.Summary = "Gets the current NHS England hours target.";
                return o;
            });
        group.MapGet("count", GetHoursCount)
            .Produces<HoursCount>()
            .Produces(StatusCodes.Status304NotModified)
            .WithOpenApi(o =>
            {
                o.Responses[StatusCodes.Status200OK.ToString()].Description = "The person-hours count matching the given query.";
                o.Responses[StatusCodes.Status200OK.ToString()].Headers.Add("Last-Modified", new() { Description = "The date and time the count was last modified.", Schema = new() { Type = "string", Format = "date-time" } });
                o.Responses[StatusCodes.Status200OK.ToString()].Content["application/json"].Example = new OpenApiObject()
                {
                    ["counts"] = new OpenApiObject
                    {
                        ["NorthEast"] = new OpenApiString("1.2:30:00"),
                        ["LondonAmbulanceService"] = new OpenApiString("1.2:30:00"),
                    },
                    ["lastUpdate"] = new OpenApiString("2021-09-01T12:00:00Z"),
                };
                o.Responses[StatusCodes.Status304NotModified.ToString()].Description = "The count has not changed since the given date.";
                o.Summary = "Gets the person-hours count matching the given query.";
                o.Parameters.Add(new() { Name = "If-Modified-Since", In = ParameterLocation.Header, Schema = new() { Type = "string", Format = "date-time" }, Required = false, Description = "If the count has not changed since this date, a 304 response will be returned." });
                o.Parameters.Add(new() { Name = "date", In = ParameterLocation.Query, Schema = new() { Type = "string", Format = "date-time" }, Required = false, Description = "A date filter for the hours entries.  Must always be a valid date, but dateType will determine which aspects are relevant.  If omitted, all hours will be returned." });
                o.Parameters.Add(new() { Name = "dateType", In = ParameterLocation.Query, Schema = new() { Type = "string", Enum = [new OpenApiString("Day"), new OpenApiString("Month"), new OpenApiString("Year")] }, Required = false, Description = "The level of detail the filter is applied at. Defaults to Year.",  });
                return o;
            });
        group.MapDelete("{id}", DeleteHours)
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi(o =>
            {
                o.Summary = "Deletes a hours entry.";
                o.Description = "Deletes the hours entry with the given ID.  Will succeed even if the entry does not exist.";
                o.Responses[StatusCodes.Status204NoContent.ToString()].Description = "The hours entry was deleted.";

                return o;
            });

        return webApp;
    }

    internal static async Task<IResult> AddHours(NewHoursEntry hours, IHoursService hoursService, HttpContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(HoursApiExtensions));
        var userId = context.User.GetNameIdentifierId() ?? "Unknown";

        await hoursService.AddAsync(hours);

        logger.LogHoursCreated(hours.PersonId, userId, hours);

        return Results.NoContent();
    }

    internal static async Task<IResult> DeleteHours(int id, IHoursService hoursService, HttpContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(HoursApiExtensions));
        var userId = context.User.GetNameIdentifierId() ?? "Unknown";

        await hoursService.DeleteAsync(id);

        logger.LogHoursDeleted(id, userId);

        return Results.NoContent();
    }

    internal static async Task<IResult> GetHoursCount(HoursQuery query, IHoursService hoursService, HttpContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(HoursApiExtensions));

        var ifModifiedSince = context.Request.GetTypedHeaders().IfModifiedSince;

        if (ifModifiedSince.HasValue)
        {
            var lastModified = await hoursService.GetLastModifiedAsync();

            var age = lastModified - ifModifiedSince;

            if (age < TimeSpan.FromSeconds(1))
            {
                logger.LogHoursCountNotModified(ifModifiedSince.Value, lastModified);

                return Results.StatusCode(StatusCodes.Status304NotModified);
            }
        }

        var count = await hoursService.CountAsync(query);

        context.Response.GetTypedHeaders().LastModified = count.LastUpdate;
        context.Response.GetTypedHeaders().CacheControl = new() { Private = true, NoCache = true };

        logger.LogHoursCountFound(count.LastUpdate, count);

        return Results.Ok(count);
    }

    internal static IResult GetHoursTarget() => TypedResults.Ok(new HoursTarget { Target = 4000 });

    [LoggerMessage(EventCodes.ItemCreated, LogLevel.Information, "An hours entry for person {id} was created by user {userId}.")]
    private static partial void LogHoursCreated(this ILogger logger, int id, string userId, [LogProperties] NewHoursEntry entry);

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "An hours entry with ID {id} was deleted by user {userId}.")]
    private static partial void LogHoursDeleted(this ILogger logger, int id, string userId);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "An hours count modified since {ifModifiedSince} was requested. It was last modified on {lastModified} and so has not been returned.")]
    private static partial void LogHoursCountNotModified(this ILogger logger, DateTimeOffset ifModifiedSince, DateTimeOffset lastModified);

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Hours count has been returned. It was last modified on {lastModified}.")]
    private static partial void LogHoursCountFound(this ILogger logger, DateTimeOffset lastModified, [LogProperties] HoursCount count);
}
