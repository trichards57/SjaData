// <copyright file="PatientCount.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Model.Patient;

public readonly record struct PatientCount
{
    public AreaDictionary<int> Counts { get; init; }

    public DateTimeOffset LastUpdate { get; init; }
}
