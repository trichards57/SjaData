// <copyright file="HubName.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Client.Model.Hubs;

/// <summary>
/// Represents the name of a hub.
/// </summary>
public readonly record struct HubName
{
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; init; }
}
