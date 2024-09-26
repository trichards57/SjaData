// <copyright file="Person.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SJAData.Data;

/// <summary>
/// Represents a person who does hours.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets or sets the date the person was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the district for the person.
    /// </summary>
    [MaxLength(100)]
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    [MaxLength(100)]
    [Required(AllowEmptyStrings = false)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hours the person has done.
    /// </summary>
    public ICollection<HoursEntry> Hours { get; set; } = [];

    /// <summary>
    /// Gets or sets the unique identifier for the person.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the person is a volunteer.
    /// </summary>
    public bool IsVolunteer { get; set; }

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    [MaxLength(100)]
    [Required(AllowEmptyStrings = false)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region for the person.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the clinical role for the person.
    /// </summary>
    [MaxLength(100)]
    [Required(AllowEmptyStrings = false)]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the person was created.
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
