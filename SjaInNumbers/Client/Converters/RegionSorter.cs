// <copyright file="RegionSorter.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Client.Converters;

public static class RegionSorter
{
    public static int SortRegion(Region region) => region switch
    {
        Region.NorthEast => 1,
        Region.NorthWest => 2,
        Region.EastMidlands => 3,
        Region.WestMidlands => 4,
        Region.EastOfEngland => 5,
        Region.London => 6,
        Region.SouthEast => 7,
        Region.SouthWest => 8,
        _ => 9,
    };
}
