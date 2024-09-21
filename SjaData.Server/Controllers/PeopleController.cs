// <copyright file="PeopleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.People;
using SjaData.Server.Services.Interfaces;
using System.Globalization;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller for managing people.
/// </summary>
/// <param name="personService">Service to manage people.</param>
/// <param name="logger">The logger for this controller.</param>
[Route("api/people")]
[ApiController]
[ApiVersion("1.0")]
public partial class PeopleController(IPersonService personService, ILogger<PeopleController> logger) : ControllerBase
{
    private readonly IPersonService personService = personService;
    private readonly ILogger<PeopleController> logger = logger;

    /// <summary>
    /// Gets the people activity reports for the given region.
    /// </summary>
    /// <param name="etag">The etag of the data held by the local cache.</param>
    /// <param name="region">The region to query for.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The activity report.</response>
    /// <response code="304">The report has not changed since the given date.</response>
    /// <response code="400">The query was invalid.</response>
    [HttpGet("reports")]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(typeof(IAsyncEnumerable<PersonReport>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReports([FromHeader(Name = "If-None-Match")] string? etag, [ModelBinder(typeof(RegionBinder))] Region region)
    {
        if (!Enum.IsDefined(region) || region == Region.Undefined)
        {
            return BadRequest("The region was not recognised.");
        }

        var date = DateOnly.FromDateTime(DateTime.Now);
        var actualEtagValue = await personService.GetPeopleReportsEtagAsync(date, region);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await personService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            LogItemNotModified("Hours trends", actualEtag);

            return StatusCode(StatusCodes.Status304NotModified);
        }

        var res = personService.GetPeopleReportsAsync(date, region);

        LogItemFound("Hours trends", lastUpdate, actualEtag);

        return Ok(res);
    }

    /// <summary>
    /// Accepts a CSV file containing person data and adds the people to the database.
    /// </summary>
    /// <param name="file">The CSV file containing the new data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The people file was accepted successfully.</response>
    /// <response code="400">The request was invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ReceivePersonFile(IFormFile file)
    {
        Request.EnableBuffering();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<PersonFileLineMap>();

        try
        {
            var updatedCount = await personService.AddPeople(csv.GetRecordsAsync<PersonFileLine>());

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [LoggerMessage(EventCodes.FileUploaded, LogLevel.Information, "An hours file has been updated.")]
    private partial void LogFileUploaded();

    [LoggerMessage(EventCodes.FileUploadFailed, LogLevel.Warning, "An hours file could not be parsed.")]
    private partial void LogFileUploadFailed(Exception ex);

    [LoggerMessage(EventCodes.FileUploadSuccess, LogLevel.Information, "{number} hours entries have been updated from the uploaded file.")]
    private partial void LogFileUploadSuccess(int number);

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "{item} has been returned. It was last modified on {lastModified} and has ETag {etag}.")]
    private partial void LogItemFound(string item, DateTimeOffset lastModified, EntityTagHeaderValue? etag = null);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "An {item} that does not match ETag {etag} was requested. It's etag matched and so has not been returned.")]
    private partial void LogItemNotModified(string item, EntityTagHeaderValue etag);
}
