// <copyright file="HoursEntry.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SjaData.Server.Data;

/// <summary>
/// Represents a single entry of hours worked.
/// </summary>
public class HoursEntry
{
    /// <summary>
    /// Gets or sets the unique identifier for the entry.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date of the entry.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the region for the entry.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the NHS Ambulance Service trust for the entry.
    /// </summary>
    public Trust Trust { get; set; }

    /// <summary>
    /// Gets or sets the number of hours worked.
    /// </summary>
    public TimeSpan Hours { get; set; }

    /// <summary>
    /// Gets or sets the ID of the person who worked the hours.
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Gets or sets the name of the person who worked the hours.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the entry was created.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date the entry was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }
}
