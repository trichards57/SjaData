// <copyright file="Trends.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Hours;

public class Trends
{
    public Dictionary<string, double> TwelveMonthAverage { get; } = [];
    public Dictionary<string, double> TwelveMonthMinusOneAverage { get; } = [];
    public Dictionary<string, double> SixMonthAverage { get; } = [];
    public Dictionary<string, double> SixMonthMinusOneAverage { get; } = [];
    public Dictionary<string, double> ThreeMonthAverage { get; } = [];
    public Dictionary<string, double> ThreeMonthMinusOneAverage { get; } = [];
    public Dictionary<string, double[]> Hours { get; } = [];
    public DateOnly ThresholdDate { get; set; }
}
