// -----------------------------------------------------------------------
// <copyright file="VorController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SjaInNumbers2.Client.Model;
using SjaInNumbers2.Client.Model.Vehicles;
using SjaInNumbers2.Client.Services.Interfaces;
using SjaInNumbers2.Controllers.Filters;

namespace SjaInNumbers2.Controllers;

/// <summary>
/// Controller for managing VOR incidents.
/// </summary>
/// <param name="vehicleService">Service to manage vehicles.</param>
[Route("api/vor")]
[ApiController]
public class VorController(IVehicleService vehicleService) : ControllerBase
{
    private readonly IVehicleService vehicleService = vehicleService;

    /// <summary>
    /// Gets the VOR statuses for a place.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <param name="place">The place to search.</param>
    /// <returns>The list of statuses for the given place.</returns>
    [HttpGet]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType<IAsyncEnumerable<VorStatus>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    public async Task<ActionResult<IAsyncEnumerable<VorStatus>>> Get([FromHeader(Name = "If-None-Match")] string? etag, [FromQuery] Region region)
    {
        var actualEtagValue = await vehicleService.GetVorStatusesEtagAsync(region);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await vehicleService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(vehicleService.GetVorStatus(region));
    }

    /// <summary>
    /// Gets the status of every vehicle nationally.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <returns>The requested vehicle status.</returns>
    [HttpGet("national")]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RevalidateCache]
    public async Task<ActionResult<IAsyncEnumerable<VehicleTypeStatus>>> GetNationalStatus([FromHeader(Name = "If-None-Match")] string? etag)
    {
        var actualEtagValue = await vehicleService.GetNationalVorStatusesEtagAsync();
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await vehicleService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(vehicleService.GetNationalStatus());
    }

    /// <summary>
    /// Gets the VOR statistics for a place.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <param name="place">The place to search.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the outcome of the action.</returns>
    [HttpGet("statistics")]
    [Authorize(Policy = "Lead")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    public async Task<ActionResult<VorStatistics?>> GetStatistics([FromHeader(Name = "If-None-Match")] string? etag, [FromQuery] Place place)
    {
        var actualEtagValue = await vehicleService.GetVorStatisticsEtagAsync(place);
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await vehicleService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(await vehicleService.GetStatisticsAsync(place));
    }

    /// <summary>
    /// Accepts VOR incidents.
    /// </summary>
    /// <param name="incidents">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.  Resolves to the outcome of the action.</returns>
    [Authorize(Policy = "Uploader")]
    [HttpPost]
    [NotCachedFilter]
    public async Task<IActionResult> Post([FromBody] IEnumerable<VorIncident> incidents)
    {
        await vehicleService.AddEntriesAsync(incidents.ToList());

        return Ok();
    }
}
