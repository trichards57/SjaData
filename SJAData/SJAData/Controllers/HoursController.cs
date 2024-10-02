// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SJAData.Client.Model;
using SJAData.Client.Model.Hours;
using SJAData.Controllers.Filters;
using SJAData.Model.Hours;
using SjaData.Server.Model;
using SJAData.Services.Interfaces;
using System.Globalization;
using System.Security.Claims;

namespace SJAData.Controllers;

[ApiController]
[Route("api/hours")]
public class HoursController(ILocalHoursService hoursService) : ControllerBase
{
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
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
