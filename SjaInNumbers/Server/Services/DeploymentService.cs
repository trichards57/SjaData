// <copyright file="DeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// Service for managing deployments.
/// </summary>
public class DeploymentService(ApplicationDbContext context) : IDeploymentService
{
    private readonly ApplicationDbContext context = context;

    /// <inheritdoc/>
    public async Task<int> AddDeploymentsAsync(IEnumerable<DeploymentsFileLine> deployments)
    {
        foreach (var deployment in deployments)
        {
            var district = await context.Districts.FirstOrDefaultAsync(d => d.Code == deployment.District);

            if (district == null)
            {
                continue;
            }

            var deploymentItem = await context.Deployments.FirstOrDefaultAsync(d => d.DipsReference == deployment.DipsNumber && d.Date == deployment.Date);

            if (deploymentItem == null)
            {
                deploymentItem = new Deployment();
                context.Deployments.Add(deploymentItem);
            }

            deploymentItem.AllWheelDriveAmbulances = deployment.AllWheelDriveAmbulances;
            deploymentItem.Date = deployment.Date;
            deploymentItem.DipsReference = deployment.DipsNumber!.Value;
            deploymentItem.DistrictId = district.Id;
            deploymentItem.FrontLineAmbulances = deployment.Ambulances;
            deploymentItem.Name = deployment.Name;
            deploymentItem.OffRoadAmbulances = deployment.OffRoadAmbulances;
            deploymentItem.LastModified = DateTime.UtcNow;
        }

        return await context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<NationalDeploymentSummary> GetNationalSummaryAsync(DateOnly startDate, DateOnly endDate)
    {
        return new NationalDeploymentSummary
        {
            Regions = (await context
                .Deployments
                .Include(d => d.District)
                .ToListAsync())
                .Where(d => d.Date >= startDate && d.Date <= endDate)
                .GroupBy(d => d.District.Region)
                .Select(r => new
                {
                    Region = r.Key,
                    Summaries = r.GroupBy(r => r.DistrictId)
                        .Select(d => new DistrictDeploymentSummary
                        {
                            DistrictId = d.Key,
                            District = d.First().District.Name,
                            Region = r.Key,
                            FrontLineAmbulances = CountVehicles(d => d.FrontLineAmbulances)(d),
                            AllWheelDriveAmbulances = CountVehicles(d => d.AllWheelDriveAmbulances)(d),
                            OffRoadAmbulances = CountVehicles(d => d.OffRoadAmbulances)(d),
                        })
                        .ToList(),
                })
                .ToDictionary(r => r.Region, r => r.Summaries),
        };
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<PeakLoads> GetPeakLoadsAsync(DateOnly startDate, DateOnly endDate)
    {
        return context.Deployments.Where(d => d.Date >= startDate && d.Date <= endDate)
            .GroupBy(d => d.District)
            .Select(d => new PeakLoads
            {
                Region = d.Key.Region,
                District = d.Key.Name,
                DistrictId = d.Key.Id,
                FrontLineAmbulances = d.Max(d => d.FrontLineAmbulances),
                AllWheelDriveAmbulances = d.Max(d => d.AllWheelDriveAmbulances),
                OffRoadAmbulances = d.Max(d => d.OffRoadAmbulances),
            })
            .AsAsyncEnumerable();
    }

    private static Func<IGrouping<int, Deployment>, Dictionary<DateOnly, int>> CountVehicles(Func<Deployment, int> selector) =>
        d => d.GroupBy(d => d.Date)
          .Select(d => new { Date = d.Key, Count = d.Sum(selector) })
          .Where(d => d.Count > 0)
          .ToDictionary(d => d.Date, d => d.Count);
}
