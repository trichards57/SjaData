// <copyright file="RegionConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SjaData.Server.Model.Converters;

/// <summary>
/// Converts a <see cref="Region"/> to and from a JSON string.
/// </summary>
public class RegionConverter : JsonConverter<Region>
{
    /// <summary>
    /// Gets the OpenAPI schema for the <see cref="Region"/> enum.
    /// </summary>
    public static OpenApiSchema Schema =>
        new()
        {
            Title = "Region",
            Type = "string",
            Enum = [
                new OpenApiString("NE"),
                new OpenApiString("NW"),
                new OpenApiString("WM"),
                new OpenApiString("EM"),
                new OpenApiString("EOE"),
                new OpenApiString("LON"),
                new OpenApiString("SE"),
                new OpenApiString("SW"),
            ],
        };

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

    /// <inheritdoc/>
    public override Region Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => FromString(reader.GetString() ?? string.Empty);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Region value, JsonSerializerOptions options)
        => writer.WriteStringValue(ToString(value));
}
