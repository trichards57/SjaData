// <copyright file="NewHub.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers.Shared.Model.Hubs;

/// <summary>
/// Represents a new hub.
/// </summary>
public readonly record struct NewHub
{
    /// <summary>
    /// Gets the ID of the district the hub is in.
    /// </summary>
    public int DistrictId { get; init; }

    /// <summary>
    /// Gets the name of the hub.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; init; }
}
