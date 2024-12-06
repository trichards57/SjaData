// <copyright file="DistrictSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Deployments;

public readonly record struct DistrictSummary
{
    public int DistrictId { get; init; }

    public string District { get; init; }

    public Region Region { get; init; }

    public Dictionary<DateOnly, int> FrontLineAmbulances { get; init; }

    public Dictionary<DateOnly, int> AllWheelDriveAmbulances { get; init; }

    public Dictionary<DateOnly, int> OffRoadAmbulances { get; init; }
}
