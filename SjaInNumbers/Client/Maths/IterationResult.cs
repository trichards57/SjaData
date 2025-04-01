// <copyright file="IterationResult.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Client.Maths;

public readonly record struct IterationResult
{
    public int DaysWithShortages { get; init; }
    public int TotalMoves { get; init; }
}
