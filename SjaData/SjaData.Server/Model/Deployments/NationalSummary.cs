﻿// <copyright file="NationalSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Deployments;

public readonly record struct NationalSummary
{
    public Dictionary<Region, List<DistrictSummary>> Regions { get; init; }
}