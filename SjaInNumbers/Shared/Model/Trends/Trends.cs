// <copyright file="Trends.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Trends;

/// <summary>
/// Represents the hours trends for a region.
/// </summary>
public readonly record struct Trends
{
    /// <summary>
    /// Gets the twelve-month average ending at <see cref="ReportDate"/>.
    /// </summary>
    public IReadOnlyDictionary<string, double> TwelveMonthAverage { get; init; }

    /// <summary>
    /// Gets the twelve-month average ending at <see cref="ReportDate"/> minus one month.
    /// </summary>
    public IReadOnlyDictionary<string, double> TwelveMonthMinusOneAverage { get; init; }

    /// <summary>
    /// Gets the six-month average ending at <see cref="ReportDate"/>.
    /// </summary>
    public IReadOnlyDictionary<string, double> SixMonthAverage { get; init; }

    /// <summary>
    /// Gets the six-month average ending at <see cref="ReportDate"/> minus one month.
    /// </summary>
    public IReadOnlyDictionary<string, double> SixMonthMinusOneAverage { get; init; }

    /// <summary>
    /// Gets the three-month average ending at <see cref="ReportDate"/>.
    /// </summary>
    public IReadOnlyDictionary<string, double> ThreeMonthAverage { get; init; }

    /// <summary>
    /// Gets the three-month average ending at <see cref="ReportDate"/> minus one month.
    /// </summary>
    public IReadOnlyDictionary<string, double> ThreeMonthMinusOneAverage { get; init; }

    /// <summary>
    /// Gets the monthly hours for the region for the twelve months ending at <see cref="ReportDate"/>.
    /// </summary>
    public IReadOnlyDictionary<string, double[]> Hours { get; init; }

    /// <summary>
    /// Gets the threshold date for the trends.
    /// </summary>
    public DateOnly ReportDate { get; init; }
}
