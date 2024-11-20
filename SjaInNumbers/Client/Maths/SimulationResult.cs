// <copyright file="SimulationResult.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Client.Maths;

public readonly record struct SimulationResult
{
    public Dictionary<int, DistrictSimulationResult> DistrictResults { get; init; }

    public double AverageAvailability { get; init; }
}
