// <copyright file="IDeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Client.Services.Interfaces;

/// <summary>
/// Represents a service for retrieving deployment data.
/// </summary>
public interface IDeploymentService
{
    /// <summary>
    /// Gets the national deployment summary.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the national deployment summary.
    /// </returns>
    Task<NationalDeploymentSummary> GetNationalSummaryAsync();

    /// <summary>
    /// Gets the national peak load reports.
    /// </summary>
    /// <returns>The list of peak loads.</returns>
    IAsyncEnumerable<PeakLoads> GetPeakLoadsAsync();
}
