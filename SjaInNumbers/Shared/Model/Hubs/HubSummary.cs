// <copyright file="HubSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Hubs;

/// <summary>
/// Represents the summary of a hub.
/// </summary>
public readonly record struct HubSummary
{
    /// <summary>
    /// Gets the ID of the hub.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the region of the hub.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the district of the hub.
    /// </summary>
    public string District { get; init; }

    /// <summary>
    /// Gets the name of the hub.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the number of vehicles in the hub.
    /// </summary>
    public int VehicleCount { get; init; }
}
