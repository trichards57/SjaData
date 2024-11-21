// <copyright file="PeakLoads.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Deployments;

/// <summary>
/// Represents the peak loads for a district.
/// </summary>
public readonly record struct PeakLoads
{
    /// <summary>
    /// Gets the region the district is in.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the name of the district.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the ID of the district.
    /// </summary>
    public int DistrictId { get; init; }

    /// <summary>
    /// Gets the peak number of front-line ambulances required.
    /// </summary>
    public int FrontLineAmbulances { get; init; }

    /// <summary>
    /// Gets the peak number of all-wheel drive ambulances required.
    /// </summary>
    public int AllWheelDriveAmbulances { get; init; }

    /// <summary>
    /// Gets the peak number of off-road ambulances required.
    /// </summary>
    public int OffRoadAmbulances { get; init; }
}
