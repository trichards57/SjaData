// <copyright file="DistrictVehicleReport.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>


// <copyright file="DistrictVehicleReport.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model;

namespace SjaInNumbers2.Client.Model.Vehicles;

/// <summary>
/// Represents the number of vehicles in the district.
/// </summary>
public readonly record struct DistrictVehicleReport
{
    /// <summary>
    /// Gets the ID of the district.
    /// </summary>
    public int DistrictId { get; init; }

    /// <summary>
    /// Gets the name of the district.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the region the district sits in.
    /// </summary>
    public Region Region { get; init; }

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
