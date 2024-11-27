// <copyright file="DistrictSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>


namespace SjaInNumbers.Shared.Model.Districts;

/// <summary>
/// Represents a summary of a district.
/// </summary>
public readonly record struct DistrictSummary : IDateMarked
{
    /// <summary>
    /// Gets the ID of the district.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the name of the district.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the region the district is in.
    /// </summary>
    public Region Region { get; init; }

    /// <summary>
    /// Gets the code of the district.
    /// </summary>
    public string Code { get; init; }

    /// <inheritdoc/>
    public DateTimeOffset LastModified { get; init; }

    /// <inheritdoc/>
    public string ETag { get; init; }
}
