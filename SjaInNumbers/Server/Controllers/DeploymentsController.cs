// <copyright file="DeploymentsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Deployments;
using System.Globalization;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller for managing deployments.
/// </summary>
[ApiController]
[Route("api/deployments")]
public class DeploymentsController(IDistrictService districtService, IDeploymentService deploymentService) : ControllerBase
{
    private readonly IDeploymentService deploymentService = deploymentService;
    private readonly IDistrictService districtService = districtService;

    /// <summary>
    /// Receives a CSV file containing deployment data and processes it.
    /// </summary>
    /// <param name="file">The uploaded deployment file.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous action.  Resolves to the result of the action.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CountResponse>> ReceiveDeploymentsFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<DeploymentsFileLineMap>();

        var updatedCount = 0;

        try
        {
            await foreach (var record in csv.GetRecordsAsync<DeploymentsFileLine>())
            {
                var district = await districtService.GetIdByDistrictCodeAsync(record.District);

                if (district == null || record.DipsNumber == 0)
                {
                    continue;
                }

                var item = new NewDeployment
                {
                    AllWheelDriveAmbulances = record.AllWheelDriveAmbulances,
                    OffRoadAmbulances = record.OffRoadAmbulances,
                    Date = record.Date,
                    DipsReference = record.DipsNumber ?? 0,
                    FrontLineAmbulances = record.Ambulances,
                    Name = record.Name,
                    DistrictId = (int)district,
                };

                await deploymentService.AddDeploymentAsync(item);
                updatedCount++;
            }

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpGet("peaks")]
    [ProducesResponseType(typeof(IEnumerable<PeakLoads>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<PeakLoads> GetPeakLoads()
    {
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddYears(-1);

        return deploymentService.GetPeakLoadsAsync(startDate, endDate);
    }
}
