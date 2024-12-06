// <copyright file="NewDeployment.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaData.Server.Model.Deployments;

public readonly record struct NewDeployment
{
    [Required(AllowEmptyStrings = false)]
    [MaxLength(100)]
    public string Name { get; init; }

    public DateOnly Date { get; init; }

    [Range(0, 100)]
    public int FrontLineAmbulances { get; init; }

    [Range(0, 100)]
    public int AllWheelDriveAmbulances { get; init; }

    [Range(0, 100)]
    public int OffRoadAmbulances { get; init; }

    [Range(0, 1000000)]
    public int DipsReference { get; init; }

    public int DistrictId { get; init; }
}
