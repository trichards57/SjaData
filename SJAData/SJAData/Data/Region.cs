// <copyright file="Region.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Data;

/// <summary>
/// Represents the SJA region.
/// </summary>
public enum Region : byte
{
    /// <summary>
    /// An undefined region.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The North East region.
    /// </summary>
    NorthEast = 1,

    /// <summary>
    /// The North West region.
    /// </summary>
    NorthWest = 2,

    /// <summary>
    /// The West Midlands region.
    /// </summary>
    WestMidlands = 3,

    /// <summary>
    /// The East Midlands region.
    /// </summary>
    EastMidlands = 4,

    /// <summary>
    /// The East of England region.
    /// </summary>
    EastOfEngland = 5,

    /// <summary>
    /// The London region.
    /// </summary>
    London = 6,

    /// <summary>
    /// The South East region.
    /// </summary>
    SouthEast = 7,

    /// <summary>
    /// The South West region.
    /// </summary>
    SouthWest = 8,
}
