// <copyright file="PatientController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.Patient;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller for managing patient records.
/// </summary>
/// <param name="patientService">Service for managing patient records.</param>
/// <param name="logger">Logger for this controller.</param>
[Route("api/patients")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "User")]
public partial class PatientController(IPatientService patientService, ILogger<PatientController> logger) : ControllerBase
{
    private readonly ILogger<PatientController> logger = logger;
    private readonly IPatientService patientService = patientService;

    /// <summary>
    /// Gets the patient count matching the given query.
    /// </summary>
    /// <param name="ifModifiedSince">The last-modified date held by the local cache.</param>
    /// <param name="region">The region to filter the patients by.</param>
    /// <param name="trust">The trust to filter the patients by.</param>
    /// <param name="eventType">The event type to filter the patients by.</param>
    /// <param name="outcome">The outcome to filter the patients by.</param>
    /// <param name="date">The date to filter the patients by.</param>
    /// <param name="dateType">The type of date filter to apply.  Defaults to month.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The patient count matching the given query.</response>
    /// <response code="304">The count has not changed since the given date.</response>
    /// <response code="400">The query was invalid.</response>
    [HttpGet("count")]
    [ProducesResponseType<PatientCount>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPatientCount(
        [FromHeader(Name = "If-Modified-Since")] DateTimeOffset? ifModifiedSince,
        [FromQuery(Name = "region")] Region? region,
        [FromQuery(Name = "trust")] Trust? trust,
        [FromQuery(Name = "event-type")] EventType? eventType,
        [FromQuery(Name = "outcome")] Outcome? outcome,
        [FromQuery(Name = "date")] DateOnly? date,
        [FromQuery(Name = "date-type")][ModelBinder(BinderType = typeof(DateTypeBinder))] DateType? dateType)
    {
        if (date is null && dateType is not null)
        {
            ModelState.AddModelError("date", "Date must be provided if date-type is specified.");
            return ValidationProblem();
        }

        if (date is not null && dateType is null)
        {
            dateType = DateType.Month;
        }

        if (region is not null and not Region.Undefined && trust is not null and not Trust.Undefined)
        {
            ModelState.AddModelError(string.Empty, "Only one of region or trust can be specified.");
            return ValidationProblem();
        }

        if (ifModifiedSince.HasValue)
        {
            var lastModified = await patientService.GetLastModifiedAsync();

            var age = lastModified - ifModifiedSince;

            if (age < TimeSpan.FromSeconds(1))
            {
                LogPatientCountNotModified(ifModifiedSince.Value, lastModified);

                return StatusCode(StatusCodes.Status304NotModified);
            }
        }

        var count = await patientService.CountAsync(region, trust, eventType, outcome, date, dateType);

        Response.GetTypedHeaders().LastModified = count.LastUpdate;
        Response.GetTypedHeaders().CacheControl = new() { Private = true, NoCache = true };

        LogPatientCountFound(count.LastUpdate, count);

        return Ok(count);
    }

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Patient count has been returned. It was last modified on {lastModified}.")]
    private partial void LogPatientCountFound(DateTimeOffset lastModified, [LogProperties] PatientCount count);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "Patient count modified since {ifModifiedSince} was requested. It was last modified on {lastModified} and so has not been returned.")]
    private partial void LogPatientCountNotModified(DateTimeOffset ifModifiedSince, DateTimeOffset lastModified);
}
