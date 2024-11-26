// <copyright file="IDeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing deployments.
/// </summary>
public interface IDeploymentService
{
    /// <summary>
    /// Adds deployments to the database.
    /// </summary>
    /// <param name="deployments">The deployments to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<int> AddDeploymentsAsync(IEnumerable<DeploymentsFileLine> deployments);

    /// <summary>
    /// Gets the national events summary.
    /// </summary>
    /// <param name="startDate">The start date to search.</param>
    /// <param name="endDate">The end date to search.</param>
    /// <returns>The national summary.</returns>
    Task<NationalDeploymentSummary> GetNationalSummaryAsync(DateOnly startDate, DateOnly endDate, string etag);

    /// <summary>
    /// Gets the list of peak loads for all districts.
    /// </summary>
    /// <param name="startDate">The start date to search.</param>
    /// <param name="endDate">The end date to search.</param>
    /// <returns>The list of peak loads.</returns>
    Task<NationalPeakLoads> GetPeakLoadsAsync(DateOnly startDate, DateOnly endDate, string etag);
}
