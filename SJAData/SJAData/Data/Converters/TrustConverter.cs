﻿// <copyright file="TrustConverter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Data.Converters;

/// <summary>
/// Converts a <see cref="Trust"/> to and from a JSON string.
/// </summary>
public static class TrustConverter
{
    /// <summary>
    /// Converts a string to a <see cref="Trust"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="Trust"/> value.</returns>
    public static Trust FromString(string value) => value switch
    {
        "NEAS" => Trust.NorthEastAmbulanceService,
        "NWAS" => Trust.NorthWestAmbulanceService,
        "WMAS" => Trust.WestMidlandsAmbulanceService,
        "EMAS" => Trust.EastMidlandsAmbulanceService,
        "EEAST" => Trust.EastOfEnglandAmbulanceService,
        "LAS" => Trust.LondonAmbulanceService,
        "SECAMB" => Trust.SouthEastCoastAmbulanceService,
        "SWAST" => Trust.SouthWesternAmbulanceService,
        "SCAS" => Trust.SouthCentralAmbulanceService,
        "YAS" => Trust.YorkshireAmbulanceService,
        "WAST" => Trust.WelshAmbulanceService,
        "SAS" => Trust.ScottishAmbulanceService,
        "NIAS" => Trust.NorthernIrelandAmbulanceService,
        "IWAS" => Trust.IsleOfWightAmbulanceService,
        _ => Trust.Undefined,
    };

    /// <summary>
    /// Gets the names of the trusts.
    /// </summary>
    /// <returns>An enumeration of names.</returns>
    public static IEnumerable<string> GetNames() => Enum.GetValues<Trust>().Where(s => s != Trust.Undefined).Select(ToString);

    /// <summary>
    /// Converts a <see cref="Trust"/> to a string.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The <see cref="Trust"/>'s initialism.</returns>
    public static string ToString(Trust value) => value switch
    {
        Trust.NorthEastAmbulanceService => "NEAS",
        Trust.NorthWestAmbulanceService => "NWAS",
        Trust.WestMidlandsAmbulanceService => "WMAS",
        Trust.EastMidlandsAmbulanceService => "EMAS",
        Trust.EastOfEnglandAmbulanceService => "EEAST",
        Trust.LondonAmbulanceService => "LAS",
        Trust.SouthEastCoastAmbulanceService => "SECAMB",
        Trust.SouthWesternAmbulanceService => "SWAST",
        Trust.SouthCentralAmbulanceService => "SCAS",
        Trust.YorkshireAmbulanceService => "YAS",
        Trust.WelshAmbulanceService => "WAST",
        Trust.ScottishAmbulanceService => "SAS",
        Trust.NorthernIrelandAmbulanceService => "NIAS",
        Trust.IsleOfWightAmbulanceService => "IWAS",
        _ => string.Empty,
    };
}
