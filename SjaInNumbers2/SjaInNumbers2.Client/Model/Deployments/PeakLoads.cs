// <copyright file="PeakLoads.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model;

namespace SjaInNumbers2.Client.Model.Deployments;

public readonly record struct PeakLoads
{
    public Region Region { get; init; }

    public string District { get; init; }

    public int DistrictId { get; init; }

    public int FrontLineAmbulances { get; init; }

    public int AllWheelDriveAmbulances { get; init; }

    public int OffRoadAmbulances { get; init; }
}
