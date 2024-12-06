// <copyright file="VehicleController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SjaData.Server.Controllers.Filters;
using SjaData.Server.Model;
using SjaData.Server.Model.Vehicles;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller for managing vehicles.
/// </summary>
/// <param name="vehicleService">Service used to manage vehicles.</param>
[Route("api/vehicles")]
[ApiController]
public class VehicleController(IVehicleService vehicleService) : ControllerBase
{
    private readonly IVehicleService vehicleService = vehicleService;

    /// <summary>
    /// A report on the national vehicle status.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <returns>The national vehicle report.</returns>
    [HttpGet("all")]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    public async Task<ActionResult<NationalVehicleReport>> GetVehiclesAsync([FromHeader(Name = "If-None-Match")] string? etag)
    {
        var actualEtagValue = await vehicleService.GetVehicleReportEtagAsync();
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await vehicleService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(await vehicleService.GetVehicleReportAsync());
    }

    /// <summary>
    /// Gets all of the vehicles for a given place.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <param name="place">The place to search.</param>
    /// <returns>The vehicle settings.</returns>
    [HttpGet]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(typeof(IAsyncEnumerable<VehicleSettings>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    public async Task<ActionResult<IAsyncEnumerable<VehicleSettings>>> GetVehiclesAsync([FromHeader(Name = "If-None-Match")] string? etag, [FromQuery] Place place)
    {
        var actualEtagValue = await vehicleService.GetSettingsEtagAsync(place);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await vehicleService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(vehicleService.GetSettingsAsync(place));
    }

    /// <summary>
    /// Gets the vehicle settings for a given vehicle.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>The vehicle settings.</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RevalidateCache]
    public async Task<ActionResult<VehicleSettings>> GetVehicleAsync([FromHeader(Name = "If-None-Match")] string? etag, int id)
    {
        var actualEtagValue = await vehicleService.GetSettingsEtagAsync(id);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await vehicleService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(Policy = "Lead")]
    [HttpPost]
    [NotCachedFilter]
    public async Task<IActionResult> PostVehicleAsync([FromBody] UpdateVehicleSettings settings)
    {
        await vehicleService.PutSettingsAsync(settings);

        return NoContent();
    }
}
