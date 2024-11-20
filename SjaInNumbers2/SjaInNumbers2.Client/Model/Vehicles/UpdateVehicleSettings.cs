// <copyright file="UpdateVehicleSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model;
using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers2.Client.Model.Vehicles;

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
    public int? HubId { get; init; }

    /// <summary>
    /// Gets the radio call sign for the vehicle.
    /// </summary>
    [Required]
    public string CallSign { get; init; }

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

    /// <summary>
    /// Gets the body type of the vehicle.
    /// </summary>
    public string BodyType { get; init; }

    /// <summary>
    /// Gets the make of the vehicle.
    /// </summary>
    public string Make { get; init; }

    /// <summary>
    /// Gets the model of the vehicle.
    /// </summary>
    public string Model { get; init; }
}
