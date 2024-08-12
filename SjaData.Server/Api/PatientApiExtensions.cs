// <copyright file="PatientApiExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Model.Patient;
using SjaData.Server.Services.Exceptions;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Api;

public static class PatientApiExtensions
{
    public static WebApplication MapPatientApi(this WebApplication app)
    {
        var group = app.MapGroup("/api/patients");

        group.MapPost(string.Empty, async ([FromBody] NewPatient patient, [FromServices] IPatientService patientService, HttpContext context) =>
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
        });

        group.MapGet("count", async (PatientQuery query, [FromServices] IPatientService patientService, HttpContext context) =>
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
            context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue { Private = true, NoCache = true };

            return Results.Ok(count);
        });

        group.MapDelete("{id}", async ([FromRoute]int id, [FromServices] IPatientService patientService) =>
        {
            await patientService.DeleteAsync(id);
            return Results.NoContent();
        });

        return app;
    }
}
