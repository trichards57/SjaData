// <copyright file="IDeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Server.Model;
using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing deployments.
/// </summary>
public interface IDeploymentService
{
    /// <summary>
    /// Adds the provided deployments.
    /// </summary>
    /// <param name="deployments">The list of deployments to add.</param>
    /// <returns>The number of added deployments.</returns>
    Task<CountResponse> AddDeployments(IAsyncEnumerable<NewDeployment> deployments);

    /// <summary>
    /// Gets the national events summary.
    /// </summary>
    /// <returns>The national summary.</returns>
    Task<NationalSummary> GetNationalSummaryAsync();

    /// <summary>
    /// Gets the list of peak loads for all districts.
    /// </summary>
    /// <returns>The list of peak loads.</returns>
    IAsyncEnumerable<PeakLoads> GetPeakLoadsAsync();
}
