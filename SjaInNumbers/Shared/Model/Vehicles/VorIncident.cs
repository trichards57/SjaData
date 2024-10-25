// <copyright file="VorIncident.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers.Shared.Model.Vehicles;

/// <summary>
/// Represents a VOR Incident reported in the VOR Report.
/// </summary>
public readonly record struct VorIncident
{
    /// <summary>
    /// Gets the vehicle's call-sign.
    /// </summary>
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the vehicle's registration.
    /// </summary>
    public string Registration { get; init; }

    /// <summary>
    /// Gets the body type of the vehicle.
    /// </summary>
    public string BodyType { get; init; }

    /// <summary>
    /// Gets the make of the vehicle.
    /// </summary>
    public string Make { get; init; }

    /// <summary>
    /// Gets the model of the vehicle.
    /// </summary>
    public string Model { get; init; }

    /// <summary>
    /// Gets the start date of the incident.
    /// </summary>
    [Required]
    public DateOnly StartDate { get; init; }

    /// <summary>
    /// Gets the description of the incident.
    /// </summary>
    public string Description { get; init; }

    /// <summary>
    /// Gets the last update of the incident.
    /// </summary>
    [Required]
    public DateOnly UpdateDate { get; init; }

    /// <summary>
    /// Gets the comments associated with the incident.
    /// </summary>
    public string Comments { get; init; }

    /// <summary>
    /// Gets the estimated date the vehicle will return to service.
    /// </summary>
    public DateOnly? EstimatedRepairDate { get; init; }
}
