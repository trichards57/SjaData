// -----------------------------------------------------------------------
// <copyright file="VorStatus.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace SjaInNumbers.Shared.Model.Vehicles;

public readonly record struct VorStatus
{
    /// <summary>
    /// Gets the internal ID for the vehicle.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the home region for the vehicle.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the home hub for the vehicle.
    /// </summary>
    public string Hub { get; init; }

    /// <summary>
    /// Gets the home district for the vehicle.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the registration of the vehicle.
    /// </summary>
    public string Registration { get; init; }

    /// <summary>
    /// Gets the call-sign for the vehicle.
    /// </summary>
    public string CallSign { get; init; }

    /// <summary>
    /// Gets the summary of the vehicle's VOR incident.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Gets a value indicating whether the vehicle is currently marked VOR.
    /// </summary>
    public bool IsVor { get; init; }

    /// <summary>
    /// Gets the date the vehicle is expected back.
    /// </summary>
    public DateOnly? DueBack { get; init; }
}
