// <copyright file="DistrictsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Districts;
using Microsoft.AspNetCore.Http;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller for managing districts.
/// </summary>
[Route("api/district")]
[ApiController]
public class DistrictsController(IDistrictService districtService) : ControllerBase
{
    private readonly IDistrictService districtService = districtService;

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
            return NotFound();
        }

        return Ok(district);
    }

    /// <summary>
    /// Gets all of the districts in the system.
    /// </summary>
    /// <returns>The list of districts.</returns>
    [HttpGet]
    public IAsyncEnumerable<DistrictSummary> GetAll() => districtService.GetAll();

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
            return NoContent();
        }

        return NotFound();
    }
}
