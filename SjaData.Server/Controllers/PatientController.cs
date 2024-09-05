// <copyright file="PatientController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using SjaData.Model.DataTypes;
using SjaData.Model.Patient;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Controllers;

[Route("api/patients")]
[ApiController]
public partial class PatientController(IPatientService patientService, ILogger<PatientController> logger) : ControllerBase
{
    private readonly IPatientService patientService = patientService;
    private readonly ILogger<PatientController> logger = logger;

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

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Patient count has been returned. It was last modified on {lastModified}.")]
    private partial void LogPatientCountFound(DateTimeOffset lastModified, [LogProperties] PatientCount count);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "Patient count modified since {ifModifiedSince} was requested. It was last modified on {lastModified} and so has not been returned.")]
    private partial void LogPatientCountNotModified(DateTimeOffset ifModifiedSince, DateTimeOffset lastModified);

    [LoggerMessage(EventCodes.ItemCreated, LogLevel.Information, "Patient record has been created by user {userId}.")]
    private partial void LogPatientCreated(string userId, [LogProperties] NewPatient patient);

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "Patient {id} deleted by user {userId}")]
    private partial void LogPatientDeleted([PatientData] int id, string userId);

}
