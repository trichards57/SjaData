// <copyright file="NewDeployment.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers.Shared.Model.Deployments;

/// <summary>
/// Represents a new deployment record.
/// </summary>
public readonly record struct NewDeployment
{
    /// <summary>
    /// Gets the name of the deployment.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; init; }

    /// <summary>
    /// Gets the date of the deployment.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Gets the number of front line ambulances required.
    /// </summary>
    [Range(0, 100)]
    public int FrontLineAmbulances { get; init; }

    /// <summary>
    /// Gets the number of all wheel drive ambulances required.
    /// </summary>
    [Range(0, 100)]
    public int AllWheelDriveAmbulances { get; init; }

    /// <summary>
    /// Gets the number of off road ambulances required.
    /// </summary>
    [Range(0, 100)]
    public int OffRoadAmbulances { get; init; }

    /// <summary>
    /// Gets the DIPS reference for the deployment.
    /// </summary>
    [Range(0, 1000000)]
    public int DipsReference { get; init; }

    /// <summary>
    /// Gets the ID of the district the deployment is for.
    /// </summary>
    public int DistrictId { get; init; }
}
