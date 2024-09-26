// <copyright file="RegionConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Data.Converters;

/// <summary>
/// Converts a <see cref="Region"/> to and from a string.
/// </summary>
public static class RegionConverter
{
    /// <summary>
    /// Converts a string to a <see cref="Region"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="Region"/> value.</returns>
    public static Region FromString(string value) => value switch
    {
        "NE" => Region.NorthEast,
        "NW" => Region.NorthWest,
        "WM" => Region.WestMidlands,
        "EM" => Region.EastMidlands,
        "EOE" => Region.EastOfEngland,
        "LON" => Region.London,
        "SE" => Region.SouthEast,
        "SW" => Region.SouthWest,
        _ => Region.Undefined,
    };

    /// <summary>
    /// Gets the names of the regions.
    /// </summary>
    /// <returns>An enumeration of names.</returns>
    public static IEnumerable<string> GetNames() => Enum.GetValues<Region>().Where(s => s != Region.Undefined).Select(ToString);

    /// <summary>
    /// Converts a <see cref="Region"/> to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="Region"/>'s initialism.</returns>
    public static string ToString(Region value) => value switch
    {
        Region.NorthEast => "NE",
        Region.NorthWest => "NW",
        Region.WestMidlands => "WM",
        Region.EastMidlands => "EM",
        Region.EastOfEngland => "EOE",
        Region.London => "LON",
        Region.SouthEast => "SE",
        Region.SouthWest => "SW",
        _ => string.Empty,
    };
}
