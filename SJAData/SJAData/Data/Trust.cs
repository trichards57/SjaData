// <copyright file="Trust.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Data;

/// <summary>
/// Represents an ambulance service trust.
/// </summary>
public enum Trust : byte
{
    /// <summary>
    /// An undefined trust.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The North East Ambulance Service.
    /// </summary>
    NorthEastAmbulanceService = 1,

    /// <summary>
    /// The North West Ambulance Service.
    /// </summary>
    NorthWestAmbulanceService = 2,

    /// <summary>
    /// The West Midlands Ambulance Service.
    /// </summary>
    WestMidlandsAmbulanceService = 3,

    /// <summary>
    /// The East Midlands Ambulance Service.
    /// </summary>
    EastMidlandsAmbulanceService = 4,

    /// <summary>
    /// The East of England Ambulance Service.
    /// </summary>
    EastOfEnglandAmbulanceService = 5,

    /// <summary>
    /// The London Ambulance Service.
    /// </summary>
    LondonAmbulanceService = 6,

    /// <summary>
    /// The South East Coast Ambulance Service.
    /// </summary>
    SouthEastCoastAmbulanceService = 7,

    /// <summary>
    /// The South Western Ambulance Service.
    /// </summary>
    SouthWesternAmbulanceService = 8,

    /// <summary>
    /// The South Central Ambulance Service.
    /// </summary>
    SouthCentralAmbulanceService = 9,

    /// <summary>
    /// The Yorkshire Ambulance Service.
    /// </summary>
    YorkshireAmbulanceService = 10,

    /// <summary>
    /// The Welsh Ambulance Service.
    /// </summary>
    WelshAmbulanceService = 11,

    /// <summary>
    /// The Scottish Ambulance Service.
    /// </summary>
    ScottishAmbulanceService = 12,

    /// <summary>
    /// The Northern Ireland Ambulance Service.
    /// </summary>
    NorthernIrelandAmbulanceService = 13,

    /// <summary>
    /// The Isle of Wight Ambulance Service.
    /// </summary>
    IsleOfWightAmbulanceService = 14,
}
