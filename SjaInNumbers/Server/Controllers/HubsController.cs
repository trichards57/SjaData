// <copyright file="HubsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Server.Controllers;

[Route("api/hubs")]
[ApiController]
[Authorize(Policy = "Lead")]
public class HubsController(IHubService hubService) : ControllerBase
{
    private readonly IHubService hubService = hubService;

    [HttpGet]
    public IAsyncEnumerable<HubSummary> GetHubSummaries() => hubService.GetAllAsync();

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
}
