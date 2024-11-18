// <copyright file="VehicleTypeStatus.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Vehicles;

/// <summary>
/// Represents the VOR status by make, model and body-type.
/// </summary>
public readonly record struct VehicleTypeStatus
{
    /// <summary>
    /// Gets the make of the vehicles.
    /// </summary>
    public string Make { get; init; }

    /// <summary>
    /// Gets the model of the vehicles.
    /// </summary>
    public string Model { get; init; }

    /// <summary>
    /// Gets the body type of the vehicles.
    /// </summary>
    public string BodyType { get; init; }

    /// <summary>
    /// Gets the number currently available.
    /// </summary>
    public int CurrentlyAvailable { get; init; }

    /// <summary>
    /// Gets the average twelve month availability rate.
    /// </summary>
    public double AverageTwelveMonthAvailability { get; init; }

    /// <summary>
    /// Gets the average twelve month minus one month availability rate.
    /// </summary>
    public double AverageTwelveMonthMinusOneAvailability { get; init; }

    /// <summary>
    /// Gets the average six month availability rate.
    /// </summary>
    public double AverageSixMonthAvailability { get; init; }

    /// <summary>
    /// Gets the average six month minus one month availability rate.
    /// </summary>
    public double AverageSixMonthMinusOneAvailability { get; init; }

    /// <summary>
    /// Gets the average three month availability rate.
    /// </summary>
    public double AverageThreeMonthAvailability { get; init; }

    /// <summary>
    /// Gets the average three month minus one month availability rate.
    /// </summary>
    public double AverageThreeMonthMinusOneAvailability { get; init; }

    /// <summary>
    /// Gets the total number of vehicles.
    /// </summary>
    public int Total { get; init; }
}
