// <copyright file="HubsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using SjaData.Server.Controllers.Filters;
using SjaData.Server.Model.Hubs;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller for managing hubs.
/// </summary>
[Route("api/hubs")]
[ApiController]
[Authorize(Policy = "Lead")]
public class HubsController(IHubService hubService) : ControllerBase
{
    private readonly IHubService hubService = hubService;

    /// <summary>
    /// Gets all of the hub summaries.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType<IAsyncEnumerable<HubSummary>>(StatusCodes.Status200OK)]
    [RevalidateCache]
    public async Task<ActionResult<IAsyncEnumerable<HubSummary>>> GetHubSummaries([FromHeader(Name = "If-None-Match")] string? etag)
    {
        var actualEtagValue = await hubService.GetAllEtagAsync();
        var actualEtag = new EntityTagHeaderValue(actualEtagValue, true);
        var etagValue = string.IsNullOrWhiteSpace(etag) ? null : EntityTagHeaderValue.Parse(etag);
        var lastUpdate = await hubService.GetLastModifiedAsync();

        Response.GetTypedHeaders().ETag = actualEtag;
        Response.GetTypedHeaders().LastModified = lastUpdate;

        if (actualEtag.Compare(etagValue, false))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        return Ok(hubService.GetAllAsync());
    }

    /// <summary>
    /// Gets the name of a hub.
    /// </summary>
    /// <param name="id">The ID of the hub whose name is required.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("{id}/name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HubName>> GetName(int id)
    {
        var hubName = await hubService.GetNameAsync(id);

        if (hubName == null)
        {
            return NotFound();
        }

        return hubName;
    }

    /// <summary>
    /// Updates the name of a hub.
    /// </summary>
    /// <param name="id">The ID of the hub whose name is being updated.</param>
    /// <param name="hubName">The new name of the hub.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost("{id}/name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PostName(int id, [FromBody] HubName hubName)
    {
        var result = await hubService.SetNameAsync(id, hubName);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Creates a new hub.
    /// </summary>
    /// <param name="newHub">The details of the new hub.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost]
    [ProducesResponseType<HubSummary>(StatusCodes.Status201Created)]
    public async Task<IActionResult> PostHub([FromBody] NewHub newHub)
    {
        var hub = await hubService.AddHubAsync(newHub);

        return Created($"/api/hubs/{hub.Id}", hub);
    }

    /// <summary>
    /// Deletes a hub.
    /// </summary>
    /// <param name="id">The ID of the hub.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteHub(int id)
    {
        try
        {
            var result = await hubService.DeleteHubAsync(id);

            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        catch (DbUpdateException)
        {
            return Conflict();
        }
    }
}
