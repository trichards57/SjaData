// <copyright file="DistrictSimulationResult.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Client.Maths;

public readonly record struct DistrictSimulationResult
{
    public int DaysShort { get; init; }

    public double DaysShortStandardDeviation { get; init; }

    public int TotalMoves { get; init; }

    public double TotalMovesStandardDeviation { get; init; }
}
