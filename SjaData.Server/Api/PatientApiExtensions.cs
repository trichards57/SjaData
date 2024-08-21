// <copyright file="PatientApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaData.Model.Patient;
using SjaData.Server.Api.Model;
using SjaData.Server.Services.Exceptions;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Api;

public static class PatientApiExtensions
{
    public static WebApplication MapPatientApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/patients");

        group.MapPost(string.Empty, AcceptPatient)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict);
        group.MapGet("count", CountPatients)
            .Produces(StatusCodes.Status304NotModified)
            .Produces<PatientCount>();
        group.MapDelete("{id}", DeletePatient)
            .Produces(StatusCodes.Status204NoContent);

        return app;
    }

    internal static async Task<IResult> AcceptPatient(NewPatient patient, IPatientService patientService, HttpContext context)
    {
        try
        {
            await patientService.AddAsync(patient);
            return Results.NoContent();
        }
        catch (DuplicateIdException)
        {
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

    internal static async Task<IResult> CountPatients(PatientQuery query, IPatientService patientService, HttpContext context)
    {
        if (context.Request.GetTypedHeaders().IfModifiedSince.HasValue)
        {
            var age = await patientService.GetLastModifiedAsync() - context.Request.GetTypedHeaders().IfModifiedSince;

            if (age < TimeSpan.FromSeconds(1))
            {
                return Results.StatusCode(StatusCodes.Status304NotModified);
            }
        }

        var count = await patientService.CountAsync(query);

        context.Response.GetTypedHeaders().LastModified = count.LastUpdate;
        context.Response.GetTypedHeaders().CacheControl = new() { Private = true, NoCache = true };

        return Results.Ok(count);
    }

    internal static async Task<IResult> DeletePatient([FromRoute] int id, [FromServices] IPatientService patientService)
    {
        await patientService.DeleteAsync(id);
        return Results.NoContent();
    }
}
