// <copyright file="DistrictDeploymentSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Deployments;

/// <summary>
/// Represents a summary of a district's deployment data.
/// </summary>
public readonly record struct DistrictDeploymentSummary
{
    /// <summary>
    /// Gets the district's ID.
    /// </summary>
    public int DistrictId { get; init; }

    /// <summary>
    /// Gets the district's name.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the region the district is in.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the number of front-line ambulances in the district on each day.
    /// </summary>
    public Dictionary<DateOnly, int> FrontLineAmbulances { get; init; }

    /// <summary>
    /// Gets the number of all-wheel drive ambulances in the district on each day.
    /// </summary>
    public Dictionary<DateOnly, int> AllWheelDriveAmbulances { get; init; }

    /// <summary>
    /// Gets the number of off-road ambulances in the district on each day.
    /// </summary>
    public Dictionary<DateOnly, int> OffRoadAmbulances { get; init; }
}
