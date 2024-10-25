// -----------------------------------------------------------------------
// <copyright file="VorStatus.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace SjaInNumbers.Shared.Model.Vehicles;

/// <summary>
/// Represents the VOR status of a vehicle.
/// </summary>
public readonly record struct VorStatus
{
    /// <summary>
    /// Gets the internal ID for the vehicle.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the home hub for the vehicle.
    /// </summary>
    public int? HubId { get; init; }

    /// <summary>
    /// Gets the home region for the vehicle.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the home district for the vehicle.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the home hub for the vehicle.
    /// </summary>
    public string Hub { get; init; }

    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    public string Registration { get; init; }

    /// <summary>
    /// Gets the call-sign for the vehicle.
    /// </summary>
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the summary of the vehicle's VOR incident.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is currently marked VOR.
    /// </summary>
    public bool IsVor { get; init; }

    /// <summary>
    /// Gets the date the vehicle is expected back.
    /// </summary>
    public DateOnly? DueBack { get; init; }

    /// <summary>
    /// Gets the percent of the time the vehicle has been VOR in the last 12 months.
    /// </summary>
    public double TwelveMonthVorCount { get; init; }

    /// <summary>
    /// Gets the percent of the time the vehicle has been VOR in the last 6 months.
    /// </summary>
    public double SixMonthVorCount { get; init; }

    /// <summary>
    /// Gets the percent of the time the vehicle has been VOR in the last 3 months.
    /// </summary>
    public double ThreeMonthVorCount { get; init; }

    /// <summary>
    /// Gets the percent of the time the vehicle has been VOR in the last 13 months, excluding the last month.
    /// </summary>
    public double TwelveMonthMinusOneVorCount { get; init; }

    /// <summary>
    /// Gets the percent of the time the vehicle has been VOR in the last 7 months, excluding the last month.
    /// </summary>
    public double SixMonthMinusOneVorCount { get; init; }

    /// <summary>
    /// Gets the percent of the time the vehicle has been VOR in the last 4 months, excluding the last month.
    /// </summary>
    public double ThreeMonthMinusOneVorCount { get; init; }

    /// <summary>
    /// Gets the list of dates the vehicle has been VOR.
    /// </summary>
    public List<DateOnly> VorDates { get; init; }
}
