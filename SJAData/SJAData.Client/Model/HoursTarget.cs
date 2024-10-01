// <copyright file="HoursTarget.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Client.Model;

public readonly record struct HoursTarget
{
    public int Target { get; init; }

    public DateOnly Date { get; init; }
}
