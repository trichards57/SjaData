// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Controllers.Filters;
using SjaData.Server.Model.Users;
using SjaData.Server.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller for managing user information.
/// </summary>
/// <param name="userService">Service to manage users.</param>
[Route("api/user")]
[ApiController]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    private readonly IUserService userService = userService;
    private readonly ILogger<UserController> logger = logger;

    /// <summary>
    /// Gets all of the current users and their roles.
    /// </summary>
    /// <returns>
    /// The list of users and their roles.
    /// </returns>
    /// <response code="200">The user list.</response>
    [HttpGet]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    [ProducesResponseType(typeof(IAsyncEnumerable<UserDetails>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<UserDetails> GetAll() => userService.GetAll();

    /// <summary>
    /// Gets the details of the current user.
    /// </summary>
    /// <returns>The result of the action.</returns>
    /// <response code="200">The current user's details.</response>
    [HttpGet("me")]
    [Authorize(Policy = "User")]
    [ProducesResponseType<CurrentUser>(StatusCodes.Status200OK)]
    public IActionResult GetMe()
    {
        var name = HttpContext.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var role = HttpContext.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        return Ok(new CurrentUser { Name = name, Role = role });
    }

    /// <summary>
    /// Updates the role of a user.
    /// </summary>
    /// <param name="userChange">The change to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="204">The user has been updated.</response>
    /// <response code="400">The request was invalid.</response>
    /// <response code="404">The user was not found.</response>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser([FromBody] UserRoleChange userChange)
    {
        var userId = HttpContext.User.GetNameIdentifierId() ?? throw new InvalidOperationException("Could not current user ID.");

        if (userId.Equals(userChange.Id, StringComparison.InvariantCulture))
        {
            return BadRequest(new ProblemDetails()
            {
                Detail = "You cannot change your own role.",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { ["traceId"] = HttpContext.TraceIdentifier },
                Instance = HttpContext.Request.Path,
                Title = "Cannot change own role",
                Type = "https://httpstatuses.com/400",
            });
        }

        if (await userService.UpdateUserAsync(userChange))
        {
            return NoContent();
        }

        return NotFound(new ProblemDetails()
        {
            Detail = "The user was not found.",
            Status = StatusCodes.Status404NotFound,
            Extensions = { ["traceId"] = HttpContext.TraceIdentifier },
            Instance = HttpContext.Request.Path,
            Title = "User not found",
            Type = "https://httpstatuses.com/404",
        });
    }
}
