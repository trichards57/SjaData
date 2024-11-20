// <copyright file="DistrictSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model;

namespace SjaInNumbers2.Client.Model.Districts;

public readonly record struct DistrictSummary
{
    public int Id { get; init; }

    public string Name { get; init; }

    public Region Region { get; init; }

    public string Code { get; init; }
}
