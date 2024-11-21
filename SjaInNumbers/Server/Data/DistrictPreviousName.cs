// <copyright file="DistrictPreviousName.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SjaInNumbers.Server.Data;

/// <summary>
/// Represents the previous names for a district.
/// </summary>
public class DistrictPreviousName
{
    /// <summary>
    /// Gets or sets the internal ID of the district.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the district the hub sits in.
    /// </summary>
    [ForeignKey(nameof(DistrictId))]
    [DeleteBehavior(DeleteBehavior.Cascade)]
    [NotNull]
    public District? District { get; set; }

    /// <summary>
    /// Gets or sets the ID of the district the hub sits in.
    /// </summary>
    public int DistrictId { get; set; }

    /// <summary>
    /// Gets or sets the previous name.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string OldName { get; set; } = string.Empty;
}
