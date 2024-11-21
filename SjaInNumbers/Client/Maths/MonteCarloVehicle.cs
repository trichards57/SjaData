// <copyright file="MonteCarloVehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MathNet.Numerics.Distributions;

namespace SjaInNumbers.Client.Maths;

/// <summary>
/// Represents a vehicle that can fail and be repaired.
/// </summary>
/// <param name="districtId">The ID of the district the vehicle is in.</param>
/// <param name="failureProbability">The probability of failure for the vehicle.</param>
/// <param name="repairTimeGenerator">The repair time generator.</param>
/// <param name="random">A source of random numbers.</param>
public class MonteCarloVehicle(int districtId, double failureProbability, IContinuousDistribution repairTimeGenerator, Random random)
{
    private readonly double failureProbability = failureProbability;
    private readonly Random random = random;
    private readonly IContinuousDistribution repairTimeGenerator = repairTimeGenerator;
    private int daysToReturn;

    public static Dictionary<int, int> RepairTimes { get; } = [];

    public int DistrictId { get; } = districtId;

    public bool IsAvailable { get; private set; }

    public int DaysAvailable { get; private set; }

    public void Reset()
    {
        IsAvailable = true;
        DaysAvailable = 0;
    }

    public void Update()
    {
        if (IsAvailable)
        {
            if (random.NextDouble() < failureProbability)
            {
                IsAvailable = false;
                daysToReturn = (int)Math.Round(repairTimeGenerator.Sample());

                if (RepairTimes.TryGetValue(daysToReturn, out int value))
                {
                    RepairTimes[daysToReturn] = value + 1;
                }
                else
                {
                    RepairTimes[daysToReturn] = 1;
                }
            }
            else
            {
                DaysAvailable++;
            }
        }
        else
        {
            if (daysToReturn == 0)
            {
                IsAvailable = true;
                DaysAvailable++;
            }
            else
            {
                daysToReturn--;
            }
        }
    }
}
