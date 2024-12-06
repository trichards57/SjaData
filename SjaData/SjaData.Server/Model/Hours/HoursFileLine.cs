// <copyright file="HoursFileLine.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Hours;

/// <summary>
/// Represents a line in the hours file.
/// </summary>
public class HoursFileLine
{
    /// <summary>
    /// Gets or sets callsign of the crew.
    /// </summary>
    public string Callsign { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets name of the crew.
    /// </summary>
    public string CrewName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets type of crew.
    /// </summary>
    public string CrewType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets crew's ID number.
    /// </summary>
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets location of the shift.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets name of the crew member.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets post of the crew member.
    /// </summary>
    public string Post { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets if the crew member is on a relief shift.
    /// </summary>
    public string Relief { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the remarks for the shift.
    /// </summary>
    public string Remarks { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets if the crew member is required.
    /// </summary>
    public string Required { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the shift.
    /// </summary>
    public string Shift { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the shift.
    /// </summary>
    public DateOnly ShiftDate { get; set; }

    /// <summary>
    /// Gets or sets the length of the shift.
    /// </summary>
    public TimeSpan ShiftLength { get; set; }
}
