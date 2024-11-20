// <copyright file="MonteCarloSimulation.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Client.Maths;

public class MonteCarloSimulation(List<MonteCarloVehicle> vehicles, Dictionary<int, Dictionary<DateOnly, int>> districtRequirements)
{
    private readonly Dictionary<int, Dictionary<DateOnly, int>> districtRequirements = districtRequirements;
    private readonly List<MonteCarloVehicle> vehicles = vehicles;

    public SimulationResult RunSimulation(DateOnly startDate, DateOnly endDate, int iterations)
    {
        var districtShortageDays = new Dictionary<int, List<int>>();
        var districtTotalMoves = new Dictionary<int, List<int>>();

        var availability = new List<double>();

        foreach (var district in districtRequirements.Keys)
        {
            districtShortageDays[district] = [];
            districtTotalMoves[district] = [];
        }

        for (int i = 0; i < iterations; i++)
        {
            foreach (var v in vehicles)
            {
                v.Reset();
            }

            var iterationResults = RunSingleIteration(startDate, endDate);

            availability.Add(vehicles.Average(v => v.DaysAvailable));

            foreach (var district in iterationResults.Keys)
            {
                districtShortageDays[district].Add(iterationResults[district].DaysWithShortages);
                districtTotalMoves[district].Add(iterationResults[district].TotalMoves);
            }
        }

        var finalResults = new Dictionary<int, DistrictSimulationResult>();

        foreach (var district in districtShortageDays.Keys)
        {
            var avgDaysWithShortages = districtShortageDays[district].Average();
            var daysStdDev = CalculateStandardDeviation(districtShortageDays[district], avgDaysWithShortages);
            var avgTotalMoves = districtTotalMoves[district].Average();
            var movesStdDev = CalculateStandardDeviation(districtTotalMoves[district], avgTotalMoves);

            finalResults[district] = new()
            {
                DaysShort = (int)Math.Round(avgDaysWithShortages),
                DaysShortStandardDeviation = daysStdDev,
                TotalMoves = (int)Math.Round(avgTotalMoves),
                TotalMovesStandardDeviation = movesStdDev,
            };
        }

        return new() { DistrictResults = finalResults, AverageAvailability = availability.Average() / 365 };
    }

    public Dictionary<int, IterationResult> RunSingleIteration(DateOnly startDate, DateOnly endDate)
    {
        var districtResults = new Dictionary<int, IterationResult>();

        foreach (var district in districtRequirements.Keys)
        {
            districtResults[district] = default;
        }

        var districtVehicles = vehicles.GroupBy(v => v.DistrictId).ToDictionary(i => i.Key, i => i.ToList());

        for (var day = startDate; day <= endDate; day = day.AddDays(1))
        {
            foreach (var v in vehicles)
            {
                v.Update();
            }

            foreach (var district in districtRequirements.Keys)
            {
                int requiredVehicles = districtRequirements[district].TryGetValue(day, out var value) ? value : 0;
                var availableVehicles = districtVehicles.TryGetValue(district, out var val) ? val.Count(v => v.IsAvailable) : 0;

                if (availableVehicles < requiredVehicles)
                {
                    var shortage = requiredVehicles - availableVehicles;
                    districtResults[district] = districtResults[district] with
                    {
                        DaysWithShortages = districtResults[district].DaysWithShortages + 1,
                        TotalMoves = districtResults[district].TotalMoves + shortage,
                    };
                }
            }
        }

        return districtResults;
    }

    private static double CalculateStandardDeviation(List<int> values, double mean)
    {
        var sumOfSquares = 0.0;

        foreach (var value in values)
        {
            sumOfSquares += Math.Pow(value - mean, 2);
        }

        return Math.Sqrt(sumOfSquares / values.Count);
    }
}
