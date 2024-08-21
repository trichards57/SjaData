// <copyright file="Hours.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Loader.Model;

internal class Hours
{
    public string Callsign { get; set; }

    public string CrewName { get; set; }

    public string CrewType { get; set; }

    public string IdNumber { get; set; }

    public string Location { get; set; }

    public string Name { get; set; }

    public string Post { get; set; }

    public string Relief { get; set; }

    public string Remarks { get; set; }

    public string Required { get; set; }

    public string Shift { get; set; }

    public DateOnly ShiftDate { get; set; }

    public TimeSpan ShiftLength { get; set; }
}
