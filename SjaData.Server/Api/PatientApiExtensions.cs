// <copyright file="PatientApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using SjaData.Model.DataTypes;
using SjaData.Model.Patient;
using SjaData.Server.Api.Model;
using SjaData.Server.Logging;
using SjaData.Server.Services.Exceptions;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Api;

/// <summary>
/// API extensions that handle patient records.
/// </summary>
public static partial class PatientApiExtensions
{
    public static WebApplication MapPatientApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/patients").WithOpenApi().WithTags("Patients");

        group.MapPost(string.Empty, AcceptPatient)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("count", GetPatientCount)
            .Produces(StatusCodes.Status304NotModified)
            .Produces<PatientCount>();
        group.MapDelete("{id}", DeletePatient)
            .Produces(StatusCodes.Status204NoContent);

        return app;
    }

    internal static async Task<IResult> AcceptPatient(NewPatient patient, IPatientService patientService, HttpContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(PatientApiExtensions));
        var userId = context.User.GetNameIdentifierId() ?? "Unknown";

        try
        {
            await patientService.AddAsync(patient);

            logger.LogPatientCreated(userId, patient);

            return Results.NoContent();
        }
        catch (DuplicateIdException)
        {
            logger.LogDuplicateIdProvided(patient.Id);

            return Results.Conflict(new ProblemDetails
            {
                Detail = "A patient with the same ID already exists.",
                Status = StatusCodes.Status409Conflict,
                Extensions =
                    {
                        ["traceId"] = context.TraceIdentifier,
                    },
                Title = "Duplicate ID",
            });
        }
    }

    internal static async Task<IResult> GetPatientCount(PatientQuery query, IPatientService patientService, HttpContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(PatientApiExtensions));

        var ifModifiedSince = context.Request.GetTypedHeaders().IfModifiedSince;

        if (ifModifiedSince.HasValue)
        {
            var lastModified = await patientService.GetLastModifiedAsync();

            var age = lastModified - ifModifiedSince;

            if (age < TimeSpan.FromSeconds(1))
            {
                logger.LogPatientCountNotModified(ifModifiedSince.Value, lastModified);

                return Results.StatusCode(StatusCodes.Status304NotModified);
            }
        }

        var count = await patientService.CountAsync(query);

        context.Response.GetTypedHeaders().LastModified = count.LastUpdate;
        context.Response.GetTypedHeaders().CacheControl = new() { Private = true, NoCache = true };

        logger.LogPatientCountFound(count.LastUpdate, count);

        return Results.Ok(count);
    }

    internal static async Task<IResult> DeletePatient(int id, IPatientService patientService, HttpContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(PatientApiExtensions));

        var userId = context.User.GetNameIdentifierId() ?? "Unknown";
        await patientService.DeleteAsync(id);

        logger.LogPatientDeleted(id, userId);

        return Results.NoContent();
    }

    [LoggerMessage(EventCodes.DuplicateIdProvided, LogLevel.Information, "A patient with the same ID {id} already exists.")]
    private static partial void LogDuplicateIdProvided(this ILogger logger, [PatientData] int id);

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Patient count has been returned. It was last modified on {lastModified}.")]
    private static partial void LogPatientCountFound(this ILogger logger, DateTimeOffset lastModified, [LogProperties] PatientCount count);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "Patient count modified since {ifModifiedSince} was requested. It was last modified on {lastModified} and so has not been returned.")]
    private static partial void LogPatientCountNotModified(this ILogger logger, DateTimeOffset ifModifiedSince, DateTimeOffset lastModified);

    [LoggerMessage(EventCodes.ItemCreated, LogLevel.Information, "Patient record has been created by user {userId}.")]
    private static partial void LogPatientCreated(this ILogger logger, string userId, [LogProperties] NewPatient patient);

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "Patient {id} deleted by user {userId}")]
    private static partial void LogPatientDeleted(this ILogger logger, [PatientData] int id, string userId);
}
