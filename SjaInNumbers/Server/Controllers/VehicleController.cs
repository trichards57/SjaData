// <copyright file="VehicleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;

namespace SjaInNumbers.Server.Controllers;

[Route("api/vehicles")]
[ApiController]
public class VehicleController(IVehicleService vehicleService) : ControllerBase
{
    private readonly IVehicleService vehicleService = vehicleService;

    /// <summary>
    /// Gets all of the vehicles for a given place.
    /// </summary>
    /// <param name="place">The place to search.</param>
    /// <returns>The vehicle settings.</returns>
    [HttpGet]
    public IAsyncEnumerable<VehicleSettings> GetVehiclesAsync([FromQuery] Place place) => vehicleService.GetSettingsAsync(place);

    /// <summary>
    /// Gets the vehicle settings for a given vehicle.
    /// </summary>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>The vehicle settings.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleSettings>> GetVehicleAsync(int id)
    {
        var vehicle = await vehicleService.GetSettingsAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return vehicle;
    }

    /// <summary>
    /// Updates the settings for a vehicle.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    /// <returns>No content.</returns>
    [HttpPost]
    public async Task<IActionResult> PostVehicleAsync([FromBody] UpdateVehicleSettings settings)
    {
        await vehicleService.PutSettingsAsync(settings);

        return NoContent();
    }
}
