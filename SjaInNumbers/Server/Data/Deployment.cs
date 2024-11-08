// <copyright file="Deployment.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Shared.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SjaInNumbers.Server.Data;

public class Deployment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    [Range(0, 100)]
    public int FrontLineAmbulances { get; set; }

    [Range(0, 100)]
    public int AllWheelDriveAmbulances { get; set; }

    [Range(0, 100)]
    public int OffRoadAmbulances { get; set; }

    [Range(0, 1000000)]
    public int DipsReference { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public int DistrictId { get; set; }

    [ForeignKey(nameof(DistrictId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public District District { get; set; }
}
