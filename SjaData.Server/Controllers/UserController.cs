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

namespace SjaData.Server.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService userService = userService;

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetMe()
    {
        var name = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        var role = HttpContext.User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { Name = name, Role = role });
    }

    [HttpGet]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    public IAsyncEnumerable<UserDetails> GetAll() => userService.GetAll();

    [HttpPost]
    [Authorize(Policy = "Admin")]
    [NotCachedFilter]
    public async Task<IActionResult> UpdateUser([FromBody] UserRoleChange userChange)
    {
        await userService.UpdateUserAsync(userChange);
        return Ok();
    }
}
