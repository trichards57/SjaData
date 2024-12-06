// <copyright file="DeploymentSummary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Deployments;

public readonly record struct DeploymentSummary
{
    public string Name { get; init; }

    public DateOnly Date { get; init; }

    public int FrontLineAmbulances { get; init; }

    public int AllWheelDriveAmbulances { get; init; }

    public int OffRoadAmbulances { get; init; }

    public Region Region { get; init; }

    public string District { get; init; }

    public int DistrictId { get; init; }
}
