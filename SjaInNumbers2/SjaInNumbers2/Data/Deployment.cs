// <copyright file="Deployment.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SjaInNumbers2.Data;

/// <summary>
/// Represents a deployment of ambulances in a district.
/// </summary>
public class Deployment
{
    /// <summary>
    /// Gets or sets the ID of the deployment.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the deployment.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the deployment.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the number of front-line ambulances in the deployment.
    /// </summary>
    [Range(0, 100)]
    public int FrontLineAmbulances { get; set; }

    /// <summary>
    /// Gets or sets the number of all-wheel drive ambulances in the deployment.
    /// </summary>
    [Range(0, 100)]
    public int AllWheelDriveAmbulances { get; set; }

    /// <summary>
    /// Gets or sets the number of off-road ambulances in the deployment.
    /// </summary>
    [Range(0, 100)]
    public int OffRoadAmbulances { get; set; }

    /// <summary>
    /// Gets or sets the DIPS reference number for the deployment.
    /// </summary>
    [Range(0, 1000000)]
    public int DipsReference { get; set; }

    /// <summary>
    /// Gets or sets the last modified date of the deployment.
    /// </summary>
    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// Gets or sets the ID of the district the deployment is in.
    /// </summary>
    public int DistrictId { get; set; }

    /// <summary>
    /// Gets or sets the district the deployment is in.
    /// </summary>
    [ForeignKey(nameof(DistrictId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    [NotNull]
    public District? District { get; set; }
}
