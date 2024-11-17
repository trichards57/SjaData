// <copyright file="MergeDistrict.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model.Districts;

/// <summary>
/// Represents a request to merge two districts.
/// </summary>
public readonly record struct MergeDistrict
{
    /// <summary>
    /// Gets the district all of the items will be taken from.
    /// </summary>
    public int SourceDistrictId { get; init; }

    /// <summary>
    /// Gets the district all of the items will be moved to.
    /// </summary>
    public int DestinationDistrictId { get; init; }
}
