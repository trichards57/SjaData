// <copyright file="MonteCarloSimulation.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers.Client.Maths;

public class MonteCarloSimulation(List<MonteCarloVehicle> vehicles, Dictionary<int, Dictionary<DateOnly, int>> districtRequirements)
{
    private readonly Dictionary<int, Dictionary<DateOnly, int>> districtRequirements = districtRequirements;
    private readonly List<MonteCarloVehicle> vehicles = vehicles;

    public Dictionary<int, SimulationResult> RunSimulation(DateOnly startDate, DateOnly endDate, int iterations)
    {
        var districtShortageDays = new Dictionary<int, List<int>>();
        var districtTotalMoves = new Dictionary<int, List<int>>();

        foreach (var district in districtRequirements.Keys)
        {
            districtShortageDays[district] = [];
            districtTotalMoves[district] = [];
        }

        for (int i = 0; i < iterations; i++)
        {
            var iterationResults = RunSingleIteration(startDate, endDate);

            foreach (var district in iterationResults.Keys)
            {
                districtShortageDays[district].Add(iterationResults[district].DaysWithShortages);
                districtTotalMoves[district].Add(iterationResults[district].TotalMoves);
            }
        }

        var finalResults = new Dictionary<int, SimulationResult>();

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

        return finalResults;
    }

    public Dictionary<int, (int DaysWithShortages, int TotalMoves)> RunSingleIteration(DateOnly startDate, DateOnly endDate)
    {
        var districtResults = new Dictionary<int, (int DaysWithShortages, int TotalMoves)>();

        foreach (var district in districtRequirements.Keys)
        {
            districtResults[district] = (DaysWithShortages: 0, TotalMoves: 0);
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
                var requiredVehicles = districtRequirements[district][day];
                var availableVehicles = districtVehicles[district].Count(v => v.IsAvailable);

                if (availableVehicles < requiredVehicles)
                {
                    var shortage = requiredVehicles - availableVehicles;
                    districtResults[district] = (districtResults[district].DaysWithShortages + 1, districtResults[district].TotalMoves + shortage);
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
