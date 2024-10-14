// <copyright file="DateType.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Shared.Model;

/// <summary>
/// Represents the type of date.
/// </summary>
public enum DateType : byte
{
    /// <summary>
    /// The date type is undefined.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The date type is a day.
    /// </summary>
    Day = 1,

    /// <summary>
    /// The date type is a month.
    /// </summary>
    Month = 2,

    /// <summary>
    /// The date type is a year.
    /// </summary>
    Year = 3,
}
