// <copyright file="FailureReport.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Client.Model.Vehicles;

public readonly record struct FailureReport
{
    public string Make { get; init; }

    public string Model { get; init; }

    public string BodyType { get; init; }

    public VehicleType VehicleType { get; init; }

    public double FailureProbability { get; init; }

    public double FailureStdDev { get; init; }
}

public readonly record struct VehicleFailureReport
{
    public double AnnualFailures { get; init; }

    public double AverageRepairTime { get; init; }

    public double GetDailyFailureProbability()
    {
        var adjustedAvailableDays = 365 - (AnnualFailures * AverageRepairTime);
        return AnnualFailures / adjustedAvailableDays;
    }

    public double GetDailyFailureVariance()
    {
        var failureProbability = GetDailyFailureProbability();
        return failureProbability * (1 - failureProbability);
    }
}
