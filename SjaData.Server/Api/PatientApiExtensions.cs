// <copyright file="PatientApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SjaData.Model.Converters;
using SjaData.Model.DataTypes;
using SjaData.Model.Patient;
using SjaData.Server.Api.Model;
using SjaData.Server.Logging;
using SjaData.Server.Services.Exceptions;
using SjaData.Server.Services.Interfaces;
using SjaData.Server.Validation;

namespace SjaData.Server.Api;

/// <summary>
/// API extensions that handle patient records.
/// </summary>
public static partial class PatientApiExtensions
{
    /// <summary>
    /// Maps the patients API to the given web application.
    /// </summary>
    /// <param name="webApp">The web application to add to.</param>
    /// <returns><paramref name="webApp"/>, to allow method chaining.</returns>
    public static WebApplication MapPatientApi(this WebApplication webApp)
    {
        var group = webApp.MapGroup("/api/patients").WithOpenApi().WithTags("Patients");

        group.MapPost(string.Empty, AcceptPatient)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithOpenApi(o =>
            {
                o.Responses[StatusCodes.Status204NoContent.ToString()].Description = "The patient record was created successfully.";
                o.Responses[StatusCodes.Status409Conflict.ToString()].Description = "The provided ID is already in use.";
                o.Summary = "Accepts a new patient record.";
                o.Description = "Accepts a new patient record. If the provided ID is already in use, a conflict will be returned.";
                return o;
            });
        group.MapGet("count", GetPatientCount)
            .Produces(StatusCodes.Status304NotModified)
            .Produces<PatientCount>()
            .WithOpenApi(o =>
            {
                o.Responses[StatusCodes.Status200OK.ToString()].Description = "The patient count matching the given query.";
                o.Responses[StatusCodes.Status200OK.ToString()].Headers.Add("Last-Modified", new() { Description = "The date and time the count was last modified.", Schema = new() { Type = "string", Format = "date-time" } });
                o.Responses[StatusCodes.Status304NotModified.ToString()].Description = "The count has not changed since the given date.";
                o.Summary = "Gets the patient count matching the given query.";
                o.Parameters.Add(new() { Name = "If-Modified-Since", In = ParameterLocation.Header, Schema = new() { Type = "string", Format = "date-time" }, Required = false, Description = "If the count has not changed since this date, a 304 response will be returned." });
                o.Parameters.Add(new() { Name = "date", In = ParameterLocation.Query, Schema = new() { Type = "string", Format = "date-time" }, Required = false, Description = "A date filter for the patient entries.  Must always be a valid date, but dateType will determine which aspects are relevant.  If omitted, all patients will be returned." });
                o.Parameters.Add(new() { Name = "dateType", In = ParameterLocation.Query, Schema = new() { Type = "string", Enum = [new OpenApiString("Day"), new OpenApiString("Month"), new OpenApiString("Year")] }, Required = false, Description = "The level of detail the filter is applied at. Defaults to Year.", });
                o.Parameters.Add(new() { Name = "eventType", In = ParameterLocation.Query, Schema = new() { Type = "string", Enum = [new OpenApiString("Event"), new OpenApiString("NHSAmbOps"), new OpenApiString("Other")] }, Required = false, Description = "The type of event to filter by.", });
                o.Parameters.Add(new() { Name = "outcome", In = ParameterLocation.Query, Schema = new() { Type = "string", Enum = [new OpenApiString("Conveyed"), new OpenApiString("NotConveyed")] }, Required = false, Description = "The type of patient outcome to filter by.", });
                o.Parameters.Add(new() { Name = "region", In = ParameterLocation.Query, Schema = RegionConverter.Schema, Required = false, Description = "The type of region to filter by.", });
                o.Parameters.Add(new() { Name = "trust", In = ParameterLocation.Query, Schema = TrustConverter.Schema, Required = false, Description = "The type of NHS trust to filter by.", });
                return o;
            });
        group.MapDelete("{id}", DeletePatient)
            .Produces(StatusCodes.Status204NoContent)
            .WithOpenApi(o =>
            {
                o.Summary = "Deletes a patient entry.";
                o.Description = "Deletes the patient entry with the given ID.  Will succeed even if the entry does not exist.";
                o.Responses[StatusCodes.Status204NoContent.ToString()].Description = "The patient entry was deleted.";

                return o;
            });

        return webApp;
    }

    internal static async Task<IResult> AcceptPatient(NewPatient patient, IPatientService patientService, HttpContext context, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(nameof(PatientApiExtensions));
        var userId = context.User.GetNameIdentifierId() ?? "Unknown";

        var validator = new NewPatientValidator();
        var result = await validator.ValidateAsync(patient);

        if (!result.IsValid)
        {
            return Results.ValidationProblem(result.Errors.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage }), "The request is invalid. Please correct the errors and try again.");
        }

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
