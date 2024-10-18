// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller for managing VOR incidents.
/// </summary>
/// <param name="vehicleService">Service to manage vehicles.</param>
[Route("api/vor")]
[ApiController]
[Authorize(Policy = "CanViewVOR")]
public class VorController(IVehicleService vehicleService) : ControllerBase
{
    private readonly IVehicleService vehicleService = vehicleService;

    /// <summary>
    /// Accepts VOR incidents.
    /// </summary>
    /// <param name="incidents">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the outcome of the action.</returns>
    [Authorize(Policy = "CanEditVOR")]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] IEnumerable<VorIncident> incidents)
    {
        await vehicleService.AddEntriesAsync(incidents.ToList());

        return Ok();
    }

    /// <summary>
    /// Gets the VOR statistics for a place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the outcome of the action.</returns>
    [HttpGet("statistics")]
    public Task<VorStatistics?> GetStatistics([FromQuery] Place place) => vehicleService.GetVorStatisticsAsync(place);

    /// <summary>
    /// Gets the VOR statuses for a place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>The list of statuses for the given place.</returns>
    [HttpGet]
    public IAsyncEnumerable<VorStatus> Get([FromQuery] Place place) => vehicleService.GetVorStatusesAsync(place);
}
