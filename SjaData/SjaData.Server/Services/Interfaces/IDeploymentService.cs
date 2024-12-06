// <copyright file="IDeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model.Deployments;

namespace SjaData.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing deployments.
/// </summary>
public interface IDeploymentService
{
    /// <summary>
    /// Adds a deployment to the database.
    /// </summary>
    /// <param name="deployment">The deployment to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddDeploymentAsync(NewDeployment deployment);

    /// <summary>
    /// Gets all of the deployments between the given dates.
    /// </summary>
    /// <param name="startDate">The start date to search.</param>
    /// <param name="endDate">The end date to search.</param>
    /// <returns>
    /// The list of deployments between the given dates.
    /// </returns>
    IAsyncEnumerable<DeploymentSummary> GetAllAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Gets the national events summary.
    /// </summary>
    /// <param name="startDate">The start date to search.</param>
    /// <param name="endDate">The end date to search.</param>
    /// <returns>The national summary.</returns>
    Task<NationalSummary> GetNationalSummaryAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Gets the list of peak loads for all districts.
    /// </summary>
    /// <param name="startDate">The start date to search.</param>
    /// <param name="endDate">The end date to search.</param>
    /// <returns>The list of peak loads.</returns>
    IAsyncEnumerable<PeakLoads> GetPeakLoadsAsync(DateOnly startDate, DateOnly endDate);
}
