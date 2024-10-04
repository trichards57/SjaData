// <copyright file="LabelConverters.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Client.Converters;

public static class LabelConverters
{
    public static string LabelToDisplay(string label) => label switch
    {
        "EastMidlands" => "East Midlands",
        "EastOfEngland" => "East of England",
        "NorthEast" => "North East",
        "NorthWest" => "North West",
        "SouthEast" => "South East",
        "SouthWest" => "South West",
        "WestMidlands" => "West Midlands",
        "London" => "London",
        "NorthEastAmbulanceService" => "North East Ambulance Service",
        "NorthWestAmbulanceService" => "North West Ambulance Service",
        "WestMidlandsAmbulanceService" => "West Midlands Ambulance Service",
        "EastMidlandsAmbulanceService" => "East Midlands Ambulance Service",
        "EastOfEnglandAmbulanceService" => "East of England Ambulance Service",
        "SouthWesternAmbulanceService" => "South Western Ambulance Service",
        "SouthCentralAmbulanceService" => "South Central Ambulance Service",
        "SouthEastCoastAmbulanceService" => "South East Coast Ambulance Service",
        "LondonAmbulanceService" => "London Ambulance Service",
        "YorkshireAmbulanceService" => "Yorkshire Ambulance Service",
        "IsleOfWightAmbulanceService" => "Isle of Wight Ambulance Service",
        _ => label,
    };

    public static bool IsTrust(string label) => label switch
    {
        "EastMidlands" => false,
        "EastOfEngland" => false,
        "NorthEast" => false,
        "NorthWest" => false,
        "SouthEast" => false,
        "SouthWest" => false,
        "WestMidlands" => false,
        "London" => false,
        "NorthEastAmbulanceService" => true,
        "NorthWestAmbulanceService" => true,
        "WestMidlandsAmbulanceService" => true,
        "EastMidlandsAmbulanceService" => true,
        "EastOfEnglandAmbulanceService" => true,
        "SouthWesternAmbulanceService" => true,
        "SouthCentralAmbulanceService" => true,
        "SouthEastCoastAmbulanceService" => true,
        "LondonAmbulanceService" => true,
        "YorkshireAmbulanceService" => true,
        "IsleOfWightAmbulanceService" => true,
        _ => true,
    };

    public static bool IsRegion(string label) => !IsTrust(label);
}
