// <copyright file="PeopleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Model;
using SjaData.Server.Model.People;
using SjaData.Server.Services.Interfaces;
using System.Globalization;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller for managing people.
/// </summary>
/// <param name="personService">Service to manage people.</param>
[Route("api/people")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "User")]
public class PeopleController(IPersonService personService) : ControllerBase
{
    private readonly IPersonService personService = personService;

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
    public async Task<IActionResult> ReceivePersonFile([FromForm] IFormFile file)
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

    [HttpGet("reports")]
    [Authorize(Policy = "Lead")]
    public async Task<ActionResult<IEnumerable<PersonReport>>> GetReports([ModelBinder(typeof(RegionBinder))] Region region)
    {
        var res = await personService.GetPeopleReports(region);
        return Ok(res.ToList());
    }
}
