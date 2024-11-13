// <copyright file="DeploymentsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using AutoMapper;
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
public partial class DeploymentsController(IDistrictService districtService, IDeploymentService deploymentService, IMapper mapper, ILogger<DeploymentsController> logger) : ControllerBase
{
    private readonly IDeploymentService deploymentService = deploymentService;
    private readonly IDistrictService districtService = districtService;
    private readonly ILogger logger = logger;
    private readonly IMapper mapper = mapper;

    /// <summary>
    /// Gets the national deployments summary.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.  Resolves to the national deployments summary.
    /// </returns>
    [HttpGet("national")]
    public async Task<ActionResult<NationalSummary>> GetNationalSummary()
    {
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddYears(-1);

        LogRequestedNationalSummary(startDate, endDate);

        return await deploymentService.GetNationalSummaryAsync(startDate, endDate);
    }

    /// <summary>
    /// Gets the peak event loads for the last year.
    /// </summary>
    /// <returns>The list of peak loads.</returns>
    [HttpGet("peaks")]
    [ProducesResponseType(typeof(IEnumerable<PeakLoads>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<PeakLoads> GetPeakLoads()
    {
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddYears(-1);

        LogRequestedPeakLoads(startDate, endDate);

        return deploymentService.GetPeakLoadsAsync(startDate, endDate);
    }

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

                await deploymentService.AddDeploymentAsync(mapper.Map<NewDeployment>(record));
                updatedCount++;
            }

            LogAddedOrUpdatedDeployments(updatedCount);

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException ex)
        {
            LogCouldNotProcessCsvData(ex);

            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [LoggerMessage(1001, LogLevel.Information, "Added or updated {numberOfDeployments} deployments.")]
    private partial void LogAddedOrUpdatedDeployments(int numberOfDeployments);

    [LoggerMessage(2001, LogLevel.Error, "Could not process the uploaded CSV data.")]
    private partial void LogCouldNotProcessCsvData(Exception exception);

    [LoggerMessage(1003, LogLevel.Information, "Requeed the national summary from {startDate} to {endDate}.")]
    private partial void LogRequestedNationalSummary(DateOnly startDate, DateOnly endDate);

    [LoggerMessage(1002, LogLevel.Information, "Requested the peak loads from {startDate} to {endDate}.")]
    private partial void LogRequestedPeakLoads(DateOnly startDate, DateOnly endDate);
}
