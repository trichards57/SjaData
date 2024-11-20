// <copyright file="HoursTarget.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Client.Model.Hours;

/// <summary>
/// Represents an hours target for a specific date.
/// </summary>
public readonly record struct HoursTarget
{
    /// <summary>
    /// Gets the hours target.
    /// </summary>
    public int Target { get; init; }

    /// <summary>
    /// Gets the identifying date for the target.
    /// </summary>
    public DateOnly Date { get; init; }
}
