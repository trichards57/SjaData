// <copyright file="District.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SjaInNumbers2.Data;

/// <summary>
/// Represents a district in a region.
/// </summary>
public class District
{
    /// <summary>
    /// Gets or sets the internal ID of the district.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the region the district is in.
    /// </summary>
    public Region Region { get; set; } = Region.Undefined;

    /// <summary>
    /// Gets or sets the name of the district.
    /// </summary>
    [MaxLength(100)]
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hubs in the district.
    /// </summary>
    public IList<Hub> Hubs { get; set; } = [];

    /// <summary>
    /// Gets or sets the districts code.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last time the district information was updated.
    /// </summary>
    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// Gets or sets the previous names of the district.
    /// </summary>
    public IList<DistrictPreviousName> PreviousNames { get; set; } = [];
}
