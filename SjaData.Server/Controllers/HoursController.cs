// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller that manages hours records.
/// </summary>
/// <param name="hoursService">Service for managing hours entries.</param>
/// <param name="logger">The logger for this controller.</param>
[ApiController]
[Route("/api/hours")]
[ApiVersion("1.0")]
[Authorize]
public partial class HoursController(IHoursService hoursService, ILogger<HoursController> logger) : ControllerBase
{
    private readonly IHoursService hoursService = hoursService;
    private readonly ILogger<HoursController> logger = logger;

    /// <summary>
    /// Accepts a new hours record.
    /// </summary>
    /// <param name="entry">The new hours information.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <remarks>
    /// If the provided person already has a shift logged on that day, it will be updated with the given information. Only one entry per person per day is allowed.
    /// </remarks>
    /// <response code="204">The hours entry was created successfully.</response>
    /// <response code="400">The request was invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddHours(NewHoursEntry entry)
    {
        await hoursService.AddAsync(entry);
        LogHoursCreated(entry.PersonId, User.GetNameIdentifierId() ?? "Unknown", entry);
        return NoContent();
    }

    /// <summary>
    /// Deletes the hours entry with the given ID.
    /// </summary>
    /// <param name="id">The ID of the entry to remove.</param>
    /// <remarks>Will succeed even if the entry does not exist.</remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="204">The hours entry was deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteHours([FromRoute] int id)
    {
        await hoursService.DeleteAsync(id);

        LogHoursDeleted(id, User.GetNameIdentifierId() ?? "Unknown");

        return NoContent();
    }

    /// <summary>
    /// Gets the person-hours count matching the given query.
    /// </summary>
    /// <param name="ifModifiedSince">The last-modified date held by the local cache.</param>
    /// <param name="date">The date to filter the hours by.</param>
    /// <param name="dateType">The type of date filter to apply.  Defaults to month.</param>
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
        [FromQuery(Name = "future")]bool future = false)
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

    [LoggerMessage(EventCodes.ItemFound, LogLevel.Information, "Hours count has been returned. It was last modified on {lastModified}.")]
    private partial void LogHoursCountFound(DateTimeOffset lastModified, [LogProperties] HoursCount count);

    [LoggerMessage(EventCodes.ItemNotModified, LogLevel.Information, "An hours count modified since {ifModifiedSince} was requested. It was last modified on {lastModified} and so has not been returned.")]
    private partial void LogHoursCountNotModified(DateTimeOffset ifModifiedSince, DateTimeOffset lastModified);

    [LoggerMessage(EventCodes.ItemCreated, LogLevel.Information, "An hours entry for person {id} was created by user {userId}.")]
    private partial void LogHoursCreated(int id, string userId, [LogProperties] NewHoursEntry entry);

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "An hours entry with ID {id} was deleted by user {userId}.")]
    private partial void LogHoursDeleted(int id, string userId);
}
