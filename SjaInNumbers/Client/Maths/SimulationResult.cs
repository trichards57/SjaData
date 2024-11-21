// <copyright file="SimulationResult.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Client.Maths;

/// <summary>
/// Represents the result of the Monte-Carlo simulation.
/// </summary>
public readonly record struct SimulationResult
{
    /// <summary>
    /// Gets the results per district.
    /// </summary>
    public Dictionary<int, DistrictSimulationResult> DistrictResults { get; init; }

    /// <summary>
    /// Gets the average vehicle availability.
    /// </summary>
    public double AverageAvailability { get; init; }
}
