// <copyright file="UserController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SjaData.Server.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var name = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        var role = HttpContext.User.FindFirstValue(ClaimTypes.Role);

        return Ok(new { Name = name, Role = role });
    }
}
