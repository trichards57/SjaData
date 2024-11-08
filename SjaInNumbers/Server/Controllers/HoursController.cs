// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SjaInNumbers.Server.Controllers.Filters;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Model.Hours;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Hours;
using SjaInNumbers.Shared.Model.Trends;
using System.Globalization;
using System.Security.Claims;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller for management of hours.
/// </summary>
/// <param name="hoursService">The service for managing hours.</param>
/// <param name="logger">The logger for this instance.</param>
[ApiController]
[Route("api/hours")]
public sealed partial class HoursController(IHoursService hoursService, ILogger<HoursController> logger) : ControllerBase
{
    private readonly ILogger logger = logger;
    private readonly IHoursService hoursService = hoursService;

    /// <summary>
    /// Gets the current NHSE Target.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("target")]
    [ProducesResponseType<HoursTarget>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    [Authorize(Policy = "Approved")]
    public async Task<ActionResult<HoursTarget>> GetTargetAsync([FromHeader(Name = "If-None-Match")] string? etag)
    {
        LogNhseTargetRequested(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown");

        var target = await hoursService.GetNhseTargetAsync();
        var actualEtagValue = await hoursService.GetNhseTargetEtagAsync();
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastModified = await hoursService.GetNhseTargetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastModified;

        if (actualEtag.Compare(etagValue, false))
        {
            LogItemNotModified(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown", "the NHSE target");
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(new HoursTarget
        {
            Target = target,
            Date = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
        });
    }

    /// <summary>
    /// Gets the count of hours around a given date.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <param name="date">The reporting date to be based on.</param>
    /// <param name="dateType">The date type to use for the report.</param>
    /// <param name="future">Should only future information be included.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("count")]
    [ProducesResponseType<HoursCount>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [RevalidateCache]
    [Authorize(Policy = "Approved")]
    public async Task<ActionResult<HoursCount>> GetHoursCount(
        [FromHeader(Name = "If-None-Match")] string? etag,
        [FromQuery(Name = "date")] DateOnly date,
        [FromQuery(Name = "date-type")] DateType dateType = DateType.Month,
        [FromQuery(Name = "future")] bool future = false)
    {
        LogHoursCountRequested(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown", date);

        var actualEtagValue = await hoursService.GetHoursCountEtagAsync(date, dateType, future);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);

        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = await hoursService.GetLastModifiedAsync();

        if (actualEtag.Compare(etagValue, false))
        {
            LogItemNotModified(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown", $"the hours count for {date}");
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var count = await hoursService.CountAsync(date, dateType, future);

        return Ok(count);
    }

    /// <summary>
    /// Accepts a CSV file of hours and processes it.
    /// </summary>
    /// <param name="file">The uploaded file data.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<CountResponse>(StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    [NotCached]
    public async Task<ActionResult<CountResponse>> ReceiveHoursFile(IFormFile file)
    {
        LogHoursFileUploaded(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown");

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<HoursFileLineMap>();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Could not get the current user.");
            var updatedCount = await hoursService.AddHours(csv.GetRecordsAsync<HoursFileLine>(), userId);

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException ex)
        {
            LogCsvReadError(ex);

            var problemDetails = new ProblemDetails()
            {
                Title = "The uploaded CSV data was invalid.",
                Status = StatusCodes.Status400BadRequest,
            };

            return BadRequest(problemDetails);
        }
    }

    /// <summary>
    /// Gets the hours trends for a given region.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <param name="region">The region to report for.</param>
    /// <param name="nhse">Should only NHSE data be included.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("trends")]
    [ProducesResponseType<Trends>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Lead")]
    [RevalidateCache]
    public async Task<ActionResult<Trends>> GetTrends([FromHeader(Name = "If-None-Match")] string? etag, Region region, bool nhse = false)
    {
        if (!Enum.IsDefined(region) || region == Region.Undefined)
        {
            return BadRequest("The region was not recognised.");
        }

        LogTrendsRequested(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown", region);

        var actualEtagValue = await hoursService.GetTrendsEtagAsync(region, nhse);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);

        var lastUpdate = await hoursService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            LogItemNotModified(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown", $"the trends for {region}");
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var trends = await hoursService.GetTrendsAsync(region, nhse);

        return Ok(trends);
    }

    [LoggerMessage("The user {UserId} has requested the NHSE target.", EventId = 1000, Level = LogLevel.Information)]
    private partial void LogNhseTargetRequested(string userId);

    [LoggerMessage("The user {UserId} has requested the hours count for {Date}.", EventId = 1001, Level = LogLevel.Information)]
    private partial void LogHoursCountRequested(string userId, DateOnly date);

    [LoggerMessage("The user {UserId} has requested the trends for {Region}.", EventId = 1002, Level = LogLevel.Information)]
    private partial void LogTrendsRequested(string userId, Region region);

    [LoggerMessage("The user {UserId} has uploaded a file of hours.", EventId = 1003, Level = LogLevel.Information)]
    private partial void LogHoursFileUploaded(string userId);

    [LoggerMessage("The user {UserId} has requested {Item} but already has the latest data.", EventId = 2001, Level = LogLevel.Information)]
    private partial void LogItemNotModified(string userId, string item);

    [LoggerMessage("There was an error reading the hours CSV file.", EventId = 3001, Level = LogLevel.Error)]
    private partial void LogCsvReadError(Exception ex);
}
