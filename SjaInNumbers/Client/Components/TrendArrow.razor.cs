// <copyright file="TrendArrow.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components;

namespace SjaInNumbers.Client.Components;

/// <summary>
/// Arrow to indicate the direction of a trend.
/// </summary>
public partial class TrendArrow
{
    public const double DefaultSignificantChange = 0.05; // 5%
    public const int DefaultSignificantHours = 24;

    [Parameter]
    public double PreviousValue { get; set; }

    [Parameter]
    public double SignificantChange { get; set; } = DefaultSignificantChange;

    [Parameter]
    public double SignificantValue { get; set; } = DefaultSignificantHours;

    [Parameter]
    public double Value { get; set; }

    private double PercentChange => ValueChange / PreviousValue;

    private double ValueChange => PreviousValue - Value;

    private string GetChangeString(double hours, double percent)
    {
        if (Math.Abs(hours) > SignificantValue)
        {
            return percent.ToString("P1");
        }

        return $"{Math.Round(hours)}";
    }
}
