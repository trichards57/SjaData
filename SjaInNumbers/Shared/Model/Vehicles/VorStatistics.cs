// -----------------------------------------------------------------------
// <copyright file="VorStatistics.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace SjaInNumbers.Shared.Model.Vehicles;

public readonly record struct VorStatistics
{
    /// <summary>
    /// Gets the total number of vehicles at a place.
    /// </summary>
    public int TotalVehicles { get; init; }

    /// <summary>
    /// Gets the number of vehicles available at a place.
    /// </summary>
    public int AvailableVehicles { get; init; }

    /// <summary>
    /// Gets the number of vehicles currently showing as VOR at a place.
    /// </summary>
    public int VorVehicles { get; init; }

    /// <summary>
    /// Gets the past availability of vehicles at a place.
    /// </summary>
    public IReadOnlyDictionary<DateOnly, int> PastAvailability { get; init; }
}
