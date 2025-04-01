// <copyright file="Capacity.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SjaInNumbers.Client.Maths;
using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Deployments;
using SjaInNumbers.Shared.Model.Vehicles;
using System.Text.Json;

namespace SjaInNumbers.Client.Pages.Deployments;

[Authorize(Policy = "Lead")]
public partial class Capacity
{
    private readonly Dictionary<int, InputModel> districtModels = [];
    private readonly Random random = new();

    // Numbers from failure analysis
    private readonly IContinuousDistribution timeToReturnGenerator = new Gamma(0.84, 0.122);

    private NationalSummary deployments;
    private IList<DistrictSummary> districts = [];
    private bool running = false;
    private NationalVehicleReport vehicles;

    [Inject]
    public required IDeploymentService DeploymentService { get; set; }

    [Inject]
    public required IVehicleService VehicleService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        vehicles = await VehicleService.GetNationalReportAsync();
        deployments = await DeploymentService.GetNationalSummary();

        districts = [.. vehicles.Districts.Select(d => new DistrictSummary(d.Region, d.DistrictId, d.District))
            .Union(deployments.Regions
                .Where(r => r.Key != Region.Undefined)
                .SelectMany(r => r.Value.Select(d => new DistrictSummary(d.Region, d.DistrictId, d.District))))];

        foreach (var id in districts.Select(d => d.DistrictId))
        {
            var vehicleReport = vehicles.Districts.FirstOrDefault(v => v.DistrictId == id);
            var capacity = vehicleReport.FrontLineAmbulances + vehicleReport.AllWheelDriveAmbulances;

            districtModels[id] = new InputModel { Vehicles = capacity };
        }
    }

    private async Task CalculateCapacity()
    {
        running = true;
        StateHasChanged();

        await Task.Run(() =>
        {
            var monteCarloVehicles = new List<MonteCarloVehicle>();

            var failureProbability = 0.0232;
            var requirements = deployments.Regions
                .SelectMany(r => r.Value.Select(d => new { d.DistrictId, d.FrontLineAmbulances }))
                .ToDictionary(r => r.DistrictId, r => r.FrontLineAmbulances);

            foreach (var id in districts.Select(d => d.DistrictId))
            {
                for (var i = 0; i < districtModels[id].Vehicles; i++)
                {
                    monteCarloVehicles.Add(new MonteCarloVehicle(id, failureProbability, timeToReturnGenerator, random));
                }
            }

            var model = new MonteCarloSimulation(monteCarloVehicles, requirements);

            var endDate = DateOnly.FromDateTime(DateTime.Today);
            var startDate = endDate.AddYears(-1);

            var result = model.RunSimulation(startDate, endDate, 30);

            foreach (var r in result.DistrictResults.Keys)
            {
                if (!districtModels.ContainsKey(r))
                {
                    continue;
                }

                districtModels[r].PredictedDaysOver = result.DistrictResults[r].DaysShort;
                districtModels[r].PredictedDaysOverMargin = result.DistrictResults[r].DaysShortStandardDeviation;
                districtModels[r].PredictedMoves = result.DistrictResults[r].TotalMoves;
                districtModels[r].PredictedMovesMargin = result.DistrictResults[r].TotalMovesStandardDeviation;
            }

            Console.WriteLine(JsonSerializer.Serialize(MonteCarloVehicle.RepairTimes));
            Console.WriteLine($"{result.AverageAvailability:p2}");

            running = false;
            StateHasChanged();
        });
    }

    private sealed record DistrictSummary
    {
        public DistrictSummary(Region region, int districtId, string district)
        {
            Region = region;
            DistrictId = districtId;
            District = district;
        }

        public int DistrictId { get; }

        public string District { get; }

        public Region Region { get; }
    }

    private sealed class InputModel
    {
        public int PredictedDaysOver { get; set; }

        public double PredictedDaysOverMargin { get; set; }

        public int PredictedMoves { get; set; }

        public double PredictedMovesMargin { get; set; }

        public int Vehicles { get; set; }
    }
}
