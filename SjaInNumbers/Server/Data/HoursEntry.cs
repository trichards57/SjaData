// <copyright file="HoursEntry.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Shared.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SjaInNumbers.Server.Data;

/// <summary>
/// Represents a single entry of hours worked.
/// </summary>
public class HoursEntry
{
    /// <summary>
    /// Gets or sets the date of the entry.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the date the entry was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the number of hours worked.
    /// </summary>
    public double Hours { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the entry.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the person who worked the hours.
    /// </summary>
    [NotNull]
    [ForeignKey(nameof(PersonId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Person Person { get; set; } = default!;

    /// <summary>
    /// Gets or sets the ID of the person who worked the hours.
    /// </summary>
    public int PersonId { get; set; }

    /// <summary>
    /// Gets or sets the region for the entry.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the NHS Ambulance Service trust for the entry.
    /// </summary>
    public Trust Trust { get; set; }

    /// <summary>
    /// Gets or sets the date the entry was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user who last updated the entry.
    /// </summary>
    [ForeignKey(nameof(UpdatedById))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public ApplicationUser UpdatedBy { get; set; } = default!;

    /// <summary>
    /// Gets or sets the user ID of the person who last updated the entry.
    /// </summary>
    [Required]
    public string UpdatedById { get; set; } = string.Empty;
}
