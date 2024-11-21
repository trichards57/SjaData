// <copyright file="FailureReport.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Vehicles;

/// <summary>
/// Represents a report of the probability of a vehicle failure.
/// </summary>
public readonly record struct FailureReport
{
    /// <summary>
    /// Gets the make of the vehicle.
    /// </summary>
    public string Make { get; init; }

    /// <summary>
    /// Gets the model of the vehicle.
    /// </summary>
    public string Model { get; init; }

    /// <summary>
    /// Gets the body type of the vehicle.
    /// </summary>
    public string BodyType { get; init; }

    /// <summary>
    /// Gets the vehicle type.
    /// </summary>
    public VehicleType VehicleType { get; init; }

    /// <summary>
    /// Gets the probability of failure.
    /// </summary>
    public double FailureProbability { get; init; }

    /// <summary>
    /// Gets the standard deviation of the failure probability.
    /// </summary>
    public double FailureStdDev { get; init; }
}

/// <summary>
/// Represents a report of the vehicle failures.
/// </summary>
public readonly record struct VehicleFailureReport
{
    /// <summary>
    /// Gets the annual number of failures.
    /// </summary>
    public double AnnualFailures { get; init; }

    /// <summary>
    /// Gets the average repair time.
    /// </summary>
    public double AverageRepairTime { get; init; }

    /// <summary>
    /// Gets the daily probability of failure.
    /// </summary>
    /// <returns>The probability as a double.</returns>
    public double GetDailyFailureProbability()
    {
        var adjustedAvailableDays = 365 - (AnnualFailures * AverageRepairTime);
        return AnnualFailures / adjustedAvailableDays;
    }

    /// <summary>
    /// Gets the daily variance of the failure probability.
    /// </summary>
    /// <returns>The variance of the probability.</returns>
    public double GetDailyFailureVariance()
    {
        var failureProbability = GetDailyFailureProbability();
        return failureProbability * (1 - failureProbability);
    }
}
