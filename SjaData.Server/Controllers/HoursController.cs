// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Controllers.Filters;
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
    /// <param name="etag">The etag of the data held by the local cache.</param>
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
    [RevalidateCache]
    public async Task<IActionResult> GetHoursCount(
        [FromHeader(Name = "If-None-Match")] string? etag,
        [FromQuery(Name = "date")] DateOnly date,
        [FromQuery(Name = "date-type")][ModelBinder(BinderType = typeof(DateTypeBinder))] DateType dateType = DateType.Month,
        [FromQuery(Name = "future")] bool future = false)
    {
        var actualEtagValue = await hoursService.GetHoursCountEtagAsync(date, dateType, future);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);

        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = await hoursService.GetLastModifiedAsync();

        if (actualEtag.Compare(etagValue, false))
        {
            LogHoursCountNotModified(actualEtag);

            return StatusCode(StatusCodes.Status304NotModified);
        }

        var count = await hoursService.CountAsync(date, dateType, future);

        LogHoursCountFound(count.LastUpdate, actualEtag, count);

        return Ok(count);
    }

    /// <summary>
    /// Gets the current NHS England hours target.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The current NHS England hours target, represented as person-hours.</response>
    [HttpGet("target")]
    [ProducesResponseType<HoursTarget>(StatusCodes.Status200OK)]
    [RevalidateCache]
    public IActionResult GetHoursTarget()
    {
        return Ok(new HoursTarget { Target = 4000 });
    }

    /// <summary>
    /// Gets the trends report for the requested region.
    /// </summary>
    /// <param name="etag">The etag of the data held by the local cache.</param>
    /// <param name="region">The region to report on.</param>
    /// <param name="nhse">Indicates if only NHSE data should be returned.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="200">The region's trends report.</response>
    /// <response code="304">The count has not changed since the given date.</response>
    /// <response code="400">The query was invalid.</response>
    [HttpGet("trends")]
    [ProducesResponseType(typeof(Trends), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Lead")]
    [RevalidateCache]
    public async Task<ActionResult<Trends>> GetTrends(
        [FromHeader(Name = "If-None-Match")] string? etag,
        [ModelBinder<RegionBinder>] Region region,
        bool nhse = false)
    {
        var actualEtagValue = await hoursService.GetTrendsEtagAsync(region, nhse);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = await hoursService.GetLastModifiedAsync();

        if (actualEtag.Compare(etagValue, false))
        {
            LogHoursCountNotModified(actualEtag);

            return StatusCode(StatusCodes.Status304NotModified);
        }

        var trends = await hoursService.GetTrendsAsync(region, nhse);

        return Ok(trends);
    }

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Hours count has been returned. It was last modified on {lastModified} and has ETag {etag}.")]
    private partial void LogHoursCountFound(DateTimeOffset lastModified, EntityTagHeaderValue etag, [LogProperties] HoursCount count);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "An hours count that does not match ETag {etag} was requested. It's etag matched and so has not been returned.")]
    private partial void LogHoursCountNotModified(EntityTagHeaderValue etag);

    [LoggerMessage(EventCodes.FileUploaded, LogLevel.Information, "An hours file has been updated.")]
    private partial void LogFileUploaded();

    [LoggerMessage(EventCodes.FileUploadSuccess, LogLevel.Information, "{number} hours entries have been updated from the uploaded file.")]
    private partial void LogFileUploadSuccess(int number);

    [LoggerMessage(EventCodes.FileUploadFailed, LogLevel.Warning, "An hours file could not be parsed.")]
    private partial void LogFileUploadFailed(Exception ex);
}
