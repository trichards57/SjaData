// -----------------------------------------------------------------------
// <copyright file="KeyDates.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace SjaData.Data;

/// <summary>
/// Represents key dates stored in the database.
/// </summary>
public class KeyDates
{
    /// <summary>
    /// Gets or sets the ID of the key dates.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date of the last update file received.
    /// </summary>
    public DateOnly LastUpdateFile { get; set; }
}
