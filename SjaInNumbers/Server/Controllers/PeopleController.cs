// <copyright file="PeopleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Model.People;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.People;
using System.Globalization;
using System.Security.Claims;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller for managing people data.
/// </summary>
/// <param name="personService">The service for managing people data.</param>
[ApiController]
[Route("api/people")]
public class PeopleController(IPersonService personService) : ControllerBase
{
    private readonly IPersonService personService = personService;

    /// <summary>
    /// Accepts a CSV file of people data and adds it to the system.
    /// </summary>
    /// <param name="file">The uploaded data file.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CountResponse>> ReceivePersonFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<PersonFileLineMap>();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Unable to get user details.");
            var updatedCount = await personService.AddPeopleAsync(csv.GetRecordsAsync<PersonFileLine>(), userId);

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }

    /// <summary>
    /// Gets the people activity reports for a specific date and region.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <param name="region">The region to report for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("reports")]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(typeof(IAsyncEnumerable<PersonReport>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IAsyncEnumerable<PersonReport>>> GetReports([FromHeader(Name = "If-None-Match")] string? etag, Region region)
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
