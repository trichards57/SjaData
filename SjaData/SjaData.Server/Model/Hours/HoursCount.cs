// <copyright file="HoursCount.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Hours;

/// <summary>
/// A count of hours for each area.
/// </summary>
public readonly record struct HoursCount
{
    /// <summary>
    /// Gets the counts of hours for each area.
    /// </summary>
    public Dictionary<string, TimeSpan> Counts { get; init; }

    /// <summary>
    /// Gets the date and time of the last update.
    /// </summary>
    public DateTimeOffset LastUpdate { get; init; }
}
