// <copyright file="PersonReport.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Client.Model.People;

/// <summary>
/// Represents a report on a person's activity.
/// </summary>
public readonly record struct PersonReport
{
    /// <summary>
    /// Gets the person's name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the number of months since the person was last active.
    /// </summary>
    public int MonthsSinceLastActive { get; init; }

    /// <summary>
    /// Gets the number of hours in the last 12 months.
    /// </summary>
    public double[] Hours { get; init; }

    /// <summary>
    /// Gets the total number of hours this year.
    /// </summary>
    public double HoursThisYear { get; init; }

    public string District { get; init; }

    public string Hub { get; init; }
}
