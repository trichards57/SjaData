// <copyright file="NationalDeploymentSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Deployments;

/// <summary>
/// Represents a summary of the national deployment data.
/// </summary>
public readonly record struct NationalDeploymentSummary : IDateMarked
{
    /// <summary>
    /// Gets the regions and their deployment summaries.
    /// </summary>
    public Dictionary<Region, List<DistrictDeploymentSummary>> Regions { get; init; }

    /// <inheritdoc/>
    public DateTimeOffset LastModified { get; init; }

    /// <inheritdoc/>
    public string ETag { get; init; }
}
