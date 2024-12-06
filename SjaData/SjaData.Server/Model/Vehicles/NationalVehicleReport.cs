// <copyright file="NationalVehicleReport.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Vehicles;

/// <summary>
/// Represents a report on the current national fleet.
/// </summary>
public readonly record struct NationalVehicleReport
{
    /// <summary>
    /// Gets the district level reports.
    /// </summary>
    public IList<DistrictVehicleReport> Districts { get; init; }

    /// <summary>
    /// Gets the number of front line vehicles in the district.
    /// </summary>
    public int FrontLineAmbulances { get; init; }

    /// <summary>
    /// Gets the number of all-wheel-drive vehicles in the district.
    /// </summary>
    public int AllWheelDriveAmbulances { get; init; }

    /// <summary>
    /// Gets the number of four-by-four vehicles in the district.
    /// </summary>
    public int OffRoadAmbulances { get; init; }
}
