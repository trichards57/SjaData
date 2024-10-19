// <copyright file="UpdateVehicleSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers.Shared.Model.Vehicles;

/// <summary>
/// Represents a settings update for a vehicle.
/// </summary>
public readonly record struct UpdateVehicleSettings
{
    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    [Required]
    public string Registration { get; init; }

    /// <summary>
    /// Gets the owning hub.
    /// </summary>
    [Required]
    public string Hub { get; init; }

    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    [Required]
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the owning district.
    /// </summary>
    [Required]
    public string District { get; init; }

    /// <summary>
    /// Gets the owning region.
    /// </summary>
    [EnumDataType(typeof(Region))]
    [Required]
    public Region Region { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    [EnumDataType(typeof(VehicleType))]
    [Required]
    public VehicleType VehicleType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is marked for disposal.
    /// </summary>
    public bool ForDisposal { get; init; }
}
