// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Controllers.Filters;
using SjaData.Server.Data;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.Hours;
using SjaData.Server.Services.Interfaces;
using System.Globalization;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller that manages hours records.
/// </summary>
/// <param name="hoursService">Service for managing hours entries.</param>
/// <param name="logger">The logger for this controller.</param>
[ApiController]
[Route("/api/hours")]
[ApiVersion("1.0")]
[Authorize(Policy = "User")]
public partial class HoursController(IHoursService hoursService, ILogger<HoursController> logger) : ControllerBase
{
    private readonly IHoursService hoursService = hoursService;
    private readonly ILogger<HoursController> logger = logger;

    /// <summary>
    /// Accepts a CSV file containing hours data and adds the hours to the database.
    /// </summary>
    /// <param name="file">The file containing hours data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The hours file was accepted successfully.</response>
    /// <response code="400">The request was invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    public async Task<IActionResult> ReceiveHoursFile([FromForm] IFormFile file)
    {
        LogFileUploaded();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<HoursFileLineMap>();

        try
        {
            var updatedCount = await hoursService.AddHours(csv.GetRecordsAsync<HoursFileLine>());

            LogFileUploadSuccess(updatedCount);

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException ex)
        {
            LogFileUploadFailed(ex);

            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Gets the person-hours count matching the given query.
    /// </summary>
    /// <param name="ifModifiedSince">The last-modified date held by the local cache.</param>
    /// <param name="date">The date to filter the hours by.  Defaults to today's date.</param>
    /// <param name="dateType">The type of date filter to apply.  Defaults to month, unless the date is blank when it defaults to year.</param>
    /// <param name="future">Indicates that only future values are wanted.  Otherwise only values from today and the past will be returned.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The person-hours count matching the given query.</response>
    /// <response code="304">The count has not changed since the given date.</response>
    /// <response code="400">The query was invalid.</response>
    [HttpGet("count")]
    [ProducesResponseType<HoursCount>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetHoursCount(
        [FromHeader(Name = "If-Modified-Since")] DateTimeOffset? ifModifiedSince,
        [FromQuery(Name = "date")] DateOnly? date,
        [FromQuery(Name = "date-type")][ModelBinder(BinderType = typeof(DateTypeBinder))] DateType? dateType,
        [FromQuery(Name = "future")] bool future = false)
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

        if (ifModifiedSince.HasValue)
        {
            var lastModified = await hoursService.GetLastModifiedAsync();

            var age = lastModified - ifModifiedSince;

            if (age < TimeSpan.FromSeconds(1))
            {
                LogHoursCountNotModified(ifModifiedSince.Value, lastModified);

                return StatusCode(StatusCodes.Status304NotModified);
            }
        }

        var count = await hoursService.CountAsync(date, dateType, future);

        Response.GetTypedHeaders().LastModified = count.LastUpdate;
        Response.GetTypedHeaders().CacheControl = new() { Private = true, NoCache = true };

        LogHoursCountFound(count.LastUpdate, count);

        return Ok(count);
    }

    /// <summary>
    /// Gets the current NHS England hours target.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The current NHS England hours target, represented as person-hours.</response>
    [HttpGet("target")]
    [ProducesResponseType<HoursTarget>(StatusCodes.Status200OK)]
    public IActionResult GetHoursTarget()
    {
        return Ok(new HoursTarget { Target = 4000 });
    }

    /// <summary>
    /// Gets the trends report for the requested region.
    /// </summary>
    /// <param name="region">The region to report on.</param>
    /// <param name="nhse">Indicates if only NHSE data should be returned.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The region's trends report.</response>
    [HttpGet("trends")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Policy = "Lead")]
    public async Task<ActionResult<Trends>> GetTrends([ModelBinder<RegionBinder>] Region region, bool nhse = false)
    {
        var trends = await hoursService.GetTrendsAsync(region, nhse);

        return Ok(trends);
    }

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Hours count has been returned. It was last modified on {lastModified}.")]
    private partial void LogHoursCountFound(DateTimeOffset lastModified, [LogProperties] HoursCount count);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "An hours count modified since {ifModifiedSince} was requested. It was last modified on {lastModified} and so has not been returned.")]
    private partial void LogHoursCountNotModified(DateTimeOffset ifModifiedSince, DateTimeOffset lastModified);

    [LoggerMessage(EventCodes.FileUploaded, LogLevel.Information, "An hours file has been updated.")]
    private partial void LogFileUploaded();

    [LoggerMessage(EventCodes.FileUploadSuccess, LogLevel.Information, "{number} hours entries have been updated from the uploaded file.")]
    private partial void LogFileUploadSuccess(int number);

    [LoggerMessage(EventCodes.FileUploadFailed, LogLevel.Warning, "An hours file could not be parsed.")]
    private partial void LogFileUploadFailed(Exception ex);
}
