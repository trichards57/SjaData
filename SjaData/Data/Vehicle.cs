// <copyright file="Vehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Shared.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SjaData.Data;

/// <summary>
/// Represents a vehicle.
/// </summary>
[Index(nameof(Registration), IsUnique = true)]
[Index(nameof(CallSign))]
public class Vehicle : IDeletableItem
{
    /// <summary>
    /// Gets or sets the vehicle's body type.
    /// </summary>
    public string BodyType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle's call-sign.
    /// </summary>
    public string CallSign { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time the vehicle was deleted.
    /// </summary>
    public DateTimeOffset? Deleted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is marked for disposal.
    /// </summary>
    public bool ForDisposal { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's home hub.
    /// </summary>
    [ForeignKey(nameof(HubId))]
    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Hub? Hub { get; set; }

    /// <summary>
    /// Gets or sets the ID of the vehicle's home hub.
    /// </summary>
    public int? HubId { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's internal ID.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Gets the incidents associated with this vehicle.
    /// </summary>
    public List<VehicleIncident> Incidents { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether the vehicle is currently on the off-the-road list.
    /// </summary>
    public bool IsVor { get; set; }

    /// <summary>
    /// Gets or sets the date and time the vehicle was last modified.
    /// </summary>
    public DateTimeOffset LastModified { get; set; }

    /// <summary>
    /// Gets or sets the vehicle's make.
    /// </summary>
    public string Make { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle's model.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle's registration.
    /// </summary>
    [MaxLength(7)]
    public string Registration { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vehicle's type.
    /// </summary>
    public VehicleType VehicleType { get; set; } = VehicleType.Unknown;
}
