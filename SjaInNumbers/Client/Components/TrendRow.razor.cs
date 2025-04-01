// <copyright file="TrendRow.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using ApexCharts;
using Microsoft.AspNetCore.Components;
using SjaInNumbers.Shared.Model.Trends;

namespace SjaInNumbers.Client.Components;

public partial class TrendRow
{
    private readonly ApexChartOptions<ChartItem> chartOptions = new()
    {
        Chart = new()
        {
            Sparkline = new()
            {
                Enabled = true,
            },
        },
        Tooltip = new()
        {
            Enabled = false,
        },
        Xaxis = new()
        {
            Type = XAxisType.Numeric,
        },
    };

    private readonly SeriesStroke seriesStroke = new()
    {
        Color = "black",
        Width = 2,
    };

    [Parameter]
    public string KeyValue { get; set; } = string.Empty;

    [Parameter]
    public string Label { get; set; } = string.Empty;

    [Parameter]
    public Trends Trends { get; set; }

    private IEnumerable<ChartItem> HoursItems => Trends.Hours[KeyValue].Select((d, i) => new ChartItem { Hours = d, Index = i }) ?? [];

    private static int Round(double value)
    {
        if (value <= 0.001)
        {
            return 0;
        }

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
        return (int)(Math.Round(value / scale, 2) * scale);
    }

    private sealed class ChartItem
    {
        public double Hours { get; set; }

        public int Index { get; set; }
    }
}
