// <copyright file="Place.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Client.Model;

/// <summary>
/// Represents a place in the SJA.
/// </summary>
public class Place
{
    /// <summary>
    /// Gets the name of the district the place is in.
    /// </summary>
    public int? DistrictId { get; init; }

    /// <summary>
    /// Gets the name of the hub the place is in.
    /// </summary>
    public int? HubId { get; init; }

    /// <summary>
    /// Gets the name of the region the place is in.
    /// </summary>
    public Region Region { get; init; } = Region.All;

    /// <summary>
    /// Converts the place to a query string.
    /// </summary>
    /// <returns>The place as a query string.</returns>
    public string CreateQuery()
    {
        if (Region == Region.All)
        {
            return string.Empty;
        }

        if (DistrictId == null)
        {
            return $"?region={Region}";
        }

        if (HubId == null)
        {
            return $"?region={Region}&districtId={DistrictId}";
        }

        return $"?region={Region}&districtId={DistrictId}&hubId={HubId}";
    }
}
