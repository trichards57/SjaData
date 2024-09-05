// <copyright file="PatientController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using SjaData.Model;
using SjaData.Model.DataTypes;
using SjaData.Model.Patient;
using SjaData.Server.Api.Model;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Logging;
using SjaData.Server.Services;
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
public partial class PatientController(IPatientService patientService, ILogger<PatientController> logger) : ControllerBase
{
    private readonly ILogger<PatientController> logger = logger;
    private readonly IPatientService patientService = patientService;

    /// <summary>
    /// Accepts a new patient record.
    /// </summary>
    /// <param name="patient">The new patient information.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <remarks>
    /// If the provided patient records has already been uploaded, it will be updated with the given information.
    /// </remarks>
    /// <response code="204">The patient entry was created successfully.</response>
    /// <response code="400">The request was invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPatient([FromBody] NewPatient patient)
    {
        await patientService.AddAsync(patient);
        LogPatientCreated(User.GetNameIdentifierId() ?? "Unknown", patient);
        return NoContent();
    }

    /// <summary>
    /// Deletes the patient entry with the given ID.
    /// </summary>
    /// <param name="id">The ID of the entry to remove.</param>
    /// <remarks>Will succeed even if the entry does not exist.</remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="204">The patient entry was deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteHours([FromRoute] int id)
    {
        await patientService.DeleteAsync(id);

        LogPatientDeleted(id, User.GetNameIdentifierId() ?? "Unknown");

        return NoContent();
    }

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

        if (region is not null && trust is not null)
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

        var count = await patientService.CountAsync(new PatientQuery
        {
            Date = date,
            DateType = dateType,
            Region = region,
            Trust = trust,
            EventType = eventType,
            Outcome = outcome,
        });

        Response.GetTypedHeaders().LastModified = count.LastUpdate;
        Response.GetTypedHeaders().CacheControl = new() { Private = true, NoCache = true };

        LogPatientCountFound(count.LastUpdate, count);

        return Ok(count);
    }

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Patient count has been returned. It was last modified on {lastModified}.")]
    private partial void LogPatientCountFound(DateTimeOffset lastModified, [LogProperties] PatientCount count);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "Patient count modified since {ifModifiedSince} was requested. It was last modified on {lastModified} and so has not been returned.")]
    private partial void LogPatientCountNotModified(DateTimeOffset ifModifiedSince, DateTimeOffset lastModified);

    [LoggerMessage(EventCodes.ItemCreated, LogLevel.Information, "Patient record has been created by user {userId}.")]
    private partial void LogPatientCreated(string userId, [LogProperties] NewPatient patient);

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "Patient {id} deleted by user {userId}")]
    private partial void LogPatientDeleted([PatientData] int id, string userId);
}