// <copyright file="DeploymentsController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SjaInNumbers.Server.Controllers.Filters;
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
public partial class DeploymentsController(IDeploymentService deploymentService, ILogger<DeploymentsController> logger) : ControllerBase
{
    private readonly IDeploymentService deploymentService = deploymentService;
    private readonly ILogger logger = logger;

    /// <summary>
    /// Gets the national deployments summary.
    /// </summary>
    /// <param name="etag">The Etag for the data currently held by the client.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.  Resolves to the national deployments summary.
    /// </returns>
    [HttpGet("national")]
    [ProducesResponseType<NationalDeploymentSummary>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    public async Task<ActionResult<NationalDeploymentSummary>> GetNationalDeploymentSummary([FromHeader(Name = "If-None-Match")] string? etag)
    {
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddYears(-1);

        LogRequestedNationalDeploymentSummary(startDate, endDate);

        return await deploymentService.GetNationalSummaryAsync(startDate, endDate, etag ?? string.Empty);
    }

    /// <summary>
    /// Gets the peak event loads for the last year.
    /// </summary>
    /// <returns>The list of peak loads.</returns>
    [HttpGet("peaks")]
    [ProducesResponseType<NationalPeakLoads>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [RevalidateCache]
    public async Task<ActionResult<NationalPeakLoads>> GetPeakLoads([FromHeader(Name = "If-None-Match")] string? etag)
    {
        var endDate = DateOnly.FromDateTime(DateTime.Today);
        var startDate = endDate.AddYears(-1);

        LogRequestedPeakLoads(startDate, endDate);

        return await deploymentService.GetPeakLoadsAsync(startDate, endDate, etag ?? string.Empty);
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
    [NotCachedFilter]
    public async Task<ActionResult<CountResponse>> ReceiveDeploymentsFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<DeploymentsFileLineMap>();

        try
        {
            var records = csv.GetRecords<DeploymentsFileLine>()
                .Where(d => d.District != null && d.DipsNumber != 0);
            var updatedCount = await deploymentService.AddDeploymentsAsync(records);

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

    [LoggerMessage(1003, LogLevel.Information, "Requested the national deployment summary from {startDate} to {endDate}.")]
    private partial void LogRequestedNationalDeploymentSummary(DateOnly startDate, DateOnly endDate);

    [LoggerMessage(1002, LogLevel.Information, "Requested the peak loads from {startDate} to {endDate}.")]
    private partial void LogRequestedPeakLoads(DateOnly startDate, DateOnly endDate);

    [LoggerMessage(3001, LogLevel.Information, "National deployment summary has not changed.")]
    private partial void LogNationalDeploymentSummaryNotChanged();
}
