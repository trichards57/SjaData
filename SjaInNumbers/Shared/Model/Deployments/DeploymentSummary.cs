// <copyright file="DeploymentSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Deployments;

/// <summary>
/// Represents a summary of a deployment.
/// </summary>
public readonly record struct DeploymentSummary
{
    /// <summary>
    /// Gets the name of the deployment.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the date of the deployment.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Gets the number of front line ambulances required.
    /// </summary>
    public int FrontLineAmbulances { get; init; }

    /// <summary>
    /// Gets the number of all-wheel drive ambulances required.
    /// </summary>
    public int AllWheelDriveAmbulances { get; init; }

    /// <summary>
    /// Gets the number of off-road ambulances required.
    /// </summary>
    public int OffRoadAmbulances { get; init; }

    /// <summary>
    /// Gets the region of the deployment.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the name of the district of the deployment.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the ID of the district of the deployment.
    /// </summary>
    public int DistrictId { get; init; }
}
