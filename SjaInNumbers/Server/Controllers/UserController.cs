﻿// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Controllers.Filters;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Users;
using System.Security.Claims;

namespace SjaInNumbers.Server.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService userService = userService;

    [HttpGet("me")]
    [Authorize]
    [NotCachedFilter]
    [ProducesResponseType(typeof(UserDetails), StatusCodes.Status200OK)]
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

    [HttpGet]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    [ProducesResponseType(typeof(IAsyncEnumerable<UserDetails>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<UserDetails> GetAll() => userService.GetAll();

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
