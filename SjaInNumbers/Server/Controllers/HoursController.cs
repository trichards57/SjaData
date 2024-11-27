// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Controllers.Filters;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Model.Hours;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Hours;
using SjaInNumbers.Shared.Model.Trends;
using SjaInNumbers.Shared.Validation;
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
public class HoursController(IHoursService hoursService, ILogger<HoursController> logger) : ControllerBase
{
    private readonly IHoursService hoursService = hoursService;
    private readonly ILogger logger = logger;

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
        => await hoursService.CountAsync(date, dateType, future, etag ?? string.Empty);

    /// <summary>
    /// Gets the current NHSE Target.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("target")]
    [ProducesResponseType<HoursTarget>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    [Authorize(Policy = "Approved")]
    public async Task<ActionResult<HoursTarget>> GetTargetAsync()
        => await hoursService.GetNhseTargetAsync();

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
    public async Task<ActionResult<Trends>> GetTrends(
        [FromHeader(Name = "If-None-Match")] string? etag,
        [RequiredAndKnownRegion] Region region,
        bool nhse = false)
        => await hoursService.GetTrendsAsync(region, nhse, etag ?? string.Empty);

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
    [NotCachedFilter]
    public async Task<ActionResult<CountResponse>> ReceiveHoursFile(IFormFile file)
    {
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
            logger.LogError(ex, "There was an error reading the CSV file.");

            var problemDetails = new ProblemDetails()
            {
                Detail = ex.Message,
                Title = "The uploaded CSV data was invalid.",
                Status = StatusCodes.Status400BadRequest,
            };

            return BadRequest(problemDetails);
        }
    }
}
