// <copyright file="VehicleSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Vehicles;

/// <summary>
/// Represents the settings for a vehicle.
/// </summary>
public readonly record struct VehicleSettings
{
    /// <summary>
    /// Gets the internal ID for the vehicle.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    public string Registration { get; init; }

    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    public string CallSign { get; init; }

    public Region Region { get; init; }

    public string District { get; init; }

    public string Hub { get; init; }

    /// <summary>
    /// Gets the owning hub.
    /// </summary>
    public int? HubId { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    public VehicleType VehicleType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is marked for disposal.
    /// </summary>
    public bool ForDisposal { get; init; }

    public string BodyType { get; init; }

    public string Make { get; init; }

    public string Model { get; init; }
}
