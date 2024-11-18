// <copyright file="Hub.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SjaInNumbers.Server.Data;

/// <summary>
/// Represents a hub where vehicles are kept.
/// </summary>
public class Hub
{
    /// <summary>
    /// Gets or sets the internal ID of the hub.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the district the hub sits in.
    /// </summary>
    [ForeignKey(nameof(DistrictId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    [NotNull]
    public District? District { get; set; }

    /// <summary>
    /// Gets or sets the ID of the district the hub sits in.
    /// </summary>
    public int DistrictId { get; set; }

    /// <summary>
    /// Gets or sets the name of the hub.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last time the hub information was updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the vehicles in the hub.
    /// </summary>
    public IList<Vehicle> Vehicles { get; set; } = [];

    /// <summary>
    /// Gets or sets the people in the hub.
    /// </summary>
    public IList<Person> People { get; set; } = [];
}
