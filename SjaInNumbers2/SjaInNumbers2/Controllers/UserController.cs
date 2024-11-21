// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers2.Client.Model.Users;
using SjaInNumbers2.Client.Services.Interfaces;
using SjaInNumbers2.Controllers.Filters;
using System.Security.Claims;

namespace SjaInNumbers2.Controllers;

/// <summary>
/// Controller for managing user information.
/// </summary>
/// <param name="userService">Service to manage users.</param>
[Route("api/user")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService userService = userService;

    /// <summary>
    /// Gets the details of the currently logged in user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("me")]
    [Authorize]
    [NotCachedFilter]
    [ProducesResponseType(typeof(UserDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDetails>> GetCurrentUser()
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Could not current user ID.");

        var user = await userService.GetUserAsync(userId);

        if (user == null)
        {
            return Unauthorized();
        }

        return user;
    }

    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <returns>
    /// The list of users in the system.
    /// </returns>
    [HttpGet]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    [ProducesResponseType(typeof(IAsyncEnumerable<UserDetails>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<UserDetails> GetAll() => userService.GetAll();

    /// <summary>
    /// Approves a user in the system.
    /// </summary>
    /// <param name="userId">The ID of the user to be approved.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost("{userId}/approve")]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveUser([FromRoute] string userId)
    {
        var result = await userService.ApproveUserAsync(userId);

        if (!result)
        {
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

        return NoContent();
    }

    /// <summary>
    /// Updates the role of a user.
    /// </summary>
    /// <param name="userChange">The user change to apply.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser([FromBody] UserRoleChange userChange)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Could not current user ID.");

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

    /// <summary>
    /// Deletes the specified user.
    /// </summary>
    /// <param name="userId">The user to delete.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpDelete("{userId}")]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUser([FromRoute] string userId)
    {
        await userService.DeleteUserAsync(userId);

        return NoContent();
    }
}
