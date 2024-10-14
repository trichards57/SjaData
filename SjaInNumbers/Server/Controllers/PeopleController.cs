﻿using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Globalization;
using System.Security.Claims;
using SjaInNumbers.Shared.Model.People;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Model.People;

namespace SjaInNumbers.Server.Controllers;

[ApiController]
[Route("api/people")]
public class PeopleController(IPersonService personService) : ControllerBase
{
    private readonly IPersonService personService = personService;

    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ReceivePersonFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<PersonFileLineMap>();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedCount = await personService.AddPeopleAsync(csv.GetRecordsAsync<PersonFileLine>(), userId);

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpGet("reports")]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(typeof(IAsyncEnumerable<PersonReport>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetReports([FromHeader(Name = "If-None-Match")] string? etag, Region region)
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
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var res = personService.GetPeopleReportsAsync(date, region);

        return Ok(res);
    }
}
