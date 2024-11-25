// <copyright file="DistrictsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller for managing districts.
/// </summary>
[Route("api/districts")]
[ApiController]
public sealed partial class DistrictsController(IDistrictService districtService, ILogger<DistrictsController> logger) : ControllerBase
{
    private readonly IDistrictService districtService = districtService;
    private readonly ILogger logger = logger;

    /// <summary>
    /// Gets the district with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the district with the specified ID.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DistrictSummary>> Get(int id)
    {
        var district = await districtService.GetDistrict(id);

        if (district == null)
        {
            LogDistrictNotFound(id);

            return NotFound();
        }

        LogRetrievedDistrictSummary(id);

        return Ok(district);
    }

    /// <summary>
    /// Gets all of the districts in the system.
    /// </summary>
    /// <returns>The list of districts.</returns>
    [HttpGet]
    public IAsyncEnumerable<DistrictSummary> GetAll()
    {
        LogRetrievedAllDistrictSummaries();

        return districtService.GetAll();
    }

    /// <summary>
    /// Updates the code for the district with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <param name="code">The district's code.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the operation.
    /// </returns>
    [HttpPost("{id}/code")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCode(int id, [FromBody] string code)
    {
        if (await districtService.SetDistrictCodeAsync(id, code))
        {
            LogDistrictCodeUpdated(id, code);

            return NoContent();
        }

        LogDistrictNotFound(id);

        return NotFound();
    }

    /// <summary>
    /// Updates the name for the district with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <param name="name">The district's name.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the operation.
    /// </returns>
    [HttpPost("{id}/name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateName(int id, [FromBody] string name)
    {
        if (await districtService.SetDistrictNameAsync(id, name))
        {
            LogDistrictNameUpdated(id, name);

            return NoContent();
        }

        LogDistrictNotFound(id);

        return NotFound();
    }

    /// <summary>
    /// Merges the source district into the destination district, including moving all of the hubs and the
    /// historic name records.
    /// </summary>
    /// <param name="mergeDistrict">The details of the merge.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the operation.
    /// </returns>
    [HttpPost("merge")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Merge([FromBody] MergeDistrict mergeDistrict)
    {
        if (await districtService.MergeDistrictsAsync(mergeDistrict.SourceDistrictId, mergeDistrict.DestinationDistrictId))
        {
            return NoContent();
        }

        return NotFound();
    }

    [LoggerMessage(1003, LogLevel.Information, "District code for {districtId} updated.")]
    private partial void LogDistrictCodeUpdated(int districtId, string newCode);

    [LoggerMessage(1004, LogLevel.Information, "District name for {districtId} updated.")]
    private partial void LogDistrictNameUpdated(int districtId, string name);

    [LoggerMessage(2001, LogLevel.Warning, "Could not find a district with the ID {districtId}.")]
    private partial void LogDistrictNotFound(int districtId);

    [LoggerMessage(1002, LogLevel.Information, "Retrieved all the district summaries.")]
    private partial void LogRetrievedAllDistrictSummaries();

    [LoggerMessage(1001, LogLevel.Information, "Retrieved the summary for district {districtId}.")]
    private partial void LogRetrievedDistrictSummary(int districtId);
}
