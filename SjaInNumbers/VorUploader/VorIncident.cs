// -----------------------------------------------------------------------
// <copyright file="VorIncident.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace VorUploader;

/// <summary>
/// Represents a VOR Incident reported in the VOR Report.
/// </summary>
public class VorIncident
{
    /// <summary>
    /// Gets the vehicle's call-sign.
    /// </summary>
    public string CallSign { get; init; } = string.Empty;

    /// <summary>
    /// Gets the vehicle's registration.
    /// </summary>
    public string Registration { get; init; } = string.Empty;

    /// <summary>
    /// Gets the body type of the vehicle.
    /// </summary>
    public string BodyType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the make of the vehicle.
    /// </summary>
    public string Make { get; init; } = string.Empty;

    /// <summary>
    /// Gets the model of the vehicle.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// Gets the start date of the incident.
    /// </summary>
    public DateOnly StartDate { get; init; }

    /// <summary>
    /// Gets the description of the incident.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the last update of the incident.
    /// </summary>
    public DateOnly UpdateDate { get; init; }

    /// <summary>
    /// Gets the comments associated with the incident.
    /// </summary>
    public string Comments { get; init; } = string.Empty;

    /// <summary>
    /// Gets the estimated date the vehicle will return to service.
    /// </summary>
    public DateOnly? EstimatedRepairDate { get; init; }
}
