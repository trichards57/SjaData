// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Security.Claims;
using SjaInNumbers.Shared.Model.Trends;
using SjaInNumbers.Shared.Model.Hours;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Server.Model.Hours;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Controllers.Filters;

namespace SjaInNumbers.Server.Controllers;

[ApiController]
[Route("api/hours")]
public class HoursController(ILocalHoursService hoursService, ILogger<HoursController> logger) : ControllerBase
{
    private readonly ILogger logger = logger;
    private readonly ILocalHoursService hoursService = hoursService;

    [HttpGet("target")]
    [ProducesResponseType<HoursTarget>(StatusCodes.Status200OK)]
    [RevalidateCache]
    [Authorize(Policy = "Approved")]
    public async Task<IActionResult> GetTargetAsync([FromHeader(Name = "If-None-Match")] string? etag)
    {
        var target = await hoursService.GetNhseTargetAsync();
        var actualEtagValue = await hoursService.GetNhseTargetEtagAsync();
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastModified = await hoursService.GetNhseTargetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastModified;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(new HoursTarget
        {
            Target = target,
            Date = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
        });
    }

    [HttpGet("count")]
    [ProducesResponseType<HoursCount>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [RevalidateCache]
    [Authorize(Policy = "Approved")]
    public async Task<IActionResult> GetHoursCount(
        [FromHeader(Name = "If-None-Match")] string? etag,
        [FromQuery(Name = "date")] DateOnly date,
        [FromQuery(Name = "date-type")] DateType dateType = DateType.Month,
        [FromQuery(Name = "future")] bool future = false)
    {
        var actualEtagValue = await hoursService.GetHoursCountEtagAsync(date, dateType, future);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);

        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = await hoursService.GetLastModifiedAsync();

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var count = await hoursService.CountAsync(date, dateType, future);

        return Ok(count);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    public async Task<IActionResult> ReceiveHoursFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<HoursFileLineMap>();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

    [HttpGet("trends")]
    [ProducesResponseType(typeof(Trends), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [Authorize(Policy = "Lead")]
    [RevalidateCache]
    public async Task<IActionResult> GetTrends([FromHeader(Name = "If-None-Match")] string? etag, Region region, bool nhse = false)
    {
        if (!Enum.IsDefined(region) || region == Region.Undefined)
        {
            return BadRequest("The region was not recognised.");
        }

        var actualEtagValue = await hoursService.GetTrendsEtagAsync(region, nhse);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);

        var lastUpdate = await hoursService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var trends = await hoursService.GetTrendsAsync(region, nhse);

        return Ok(trends);
    }
}
