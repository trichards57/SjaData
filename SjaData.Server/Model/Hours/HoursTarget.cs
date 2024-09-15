// <copyright file="HoursTarget.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Hours;

/// <summary>
/// A target for the number of hours to work.
/// </summary>
public readonly record struct HoursTarget
{
    /// <summary>
    /// Gets the target number of hours.
    /// </summary>
    public int Target { get; init; }
}
