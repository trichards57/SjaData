// <copyright file="PatientCount.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Patient;

/// <summary>
/// Represents the number of patients in each area.
/// </summary>
public readonly record struct PatientCount
{
    /// <summary>
    /// Gets the counts of patients in each area.
    /// </summary>
    public AreaDictionary<int> Counts { get; init; }

    /// <summary>
    /// Gets the date and time of the last update.
    /// </summary>
    public DateTimeOffset LastUpdate { get; init; }
}
