﻿// <copyright file="HoursCount.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Model.Hours;

public readonly record struct HoursCount
{
    public AreaDictionary<TimeSpan> Counts { get; init; }

    public DateTimeOffset LastUpdate { get; init; }
}
