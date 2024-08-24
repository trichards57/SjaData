// <copyright file="NewHoursEntry.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model.Validation;
using System.ComponentModel.DataAnnotations;

namespace SjaData.Model.Hours;

/// <summary>
/// Represents a new hours entry.
/// </summary>
[RegionOrTrust]
public readonly record struct NewHoursEntry : IRegionAndTrust
{
    /// <summary>
    /// Gets the date of the entry.
    /// </summary>
    [Required]
    public DateOnly Date { get; init; }

    /// <summary>
    /// Gets the region for the entry.
    /// </summary>
    [EnumDataType(typeof(Region))]
    public Region Region { get; init; }

    /// <summary>
    /// Gets the NHS Ambulance Service trust for the entry.
    /// </summary>
    [EnumDataType(typeof(Trust))]
    public Trust Trust { get; init; }

    /// <summary>
    /// Gets the number of hours worked.
    /// </summary>
    [Required]
    [GreaterThan(0)]
    public TimeSpan Hours { get; init; }

    /// <summary>
    /// Gets the ID of the person who worked the hours.
    /// </summary>
    [Required]
    [GreaterThan(0)]
    public int PersonId { get; init; }

    /// <summary>
    /// Gets the name of the person who worked the hours.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(100)]
    public string Name { get; init; }
}
