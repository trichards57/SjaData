// <copyright file="IterationResult.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Client.Maths;

/// <summary>
/// Represents the result of a Monte-Carlo iteration.
/// </summary>
public readonly record struct IterationResult
{
    /// <summary>
    /// Gets the number of days with shortages.
    /// </summary>
    public int DaysWithShortages { get; init; }

    /// <summary>
    /// Gets the total number of moves made.
    /// </summary>
    public int TotalMoves { get; init; }
}
