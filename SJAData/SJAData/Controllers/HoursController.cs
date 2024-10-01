// <copyright file="HoursController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SJAData.Client.Model;
using SJAData.Client.Services.Interfaces;

namespace SJAData.Controllers;

[ApiController]
[Route("api/hours")]
public class HoursController(IHoursService hoursService) : ControllerBase
{
    private readonly IHoursService hoursService = hoursService;

    [HttpGet("target")]
    public async Task<IActionResult> GetTargetAsync()
    {
        var target = await hoursService.GetNhseTargetAsync();

        return Ok(new HoursTarget
        {
            Target = target,
            Date = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1),
        });
    }
}
