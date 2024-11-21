// <copyright file="DeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers2.Client.Model.Deployments;
using SjaInNumbers2.Client.Services.Interfaces;
using SjaInNumbers2.Data;

namespace SjaInNumbers2.Services;

/// <summary>
/// Service for managing deployments.
/// </summary>
public class DeploymentService(IDbContextFactory<ApplicationDbContext> contextBuilder, TimeProvider timeProvider) : IDeploymentService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextBuilder = contextBuilder;
    private readonly TimeProvider timeProvider = timeProvider;

    /// <inheritdoc/>
    public async Task AddDeploymentAsync(NewDeployment deployment)
    {
        using var context = await contextBuilder.CreateDbContextAsync();
        var deploymentItem = await context.Deployments.FirstOrDefaultAsync(d => d.DipsReference == deployment.DipsReference && d.Date == deployment.Date);

        if (deploymentItem == null)
        {
            deploymentItem = new Deployment();
            context.Deployments.Add(deploymentItem);
        }

        deploymentItem.AllWheelDriveAmbulances = deployment.AllWheelDriveAmbulances;
        deploymentItem.Date = deployment.Date;
        deploymentItem.DipsReference = deployment.DipsReference;
        deploymentItem.DistrictId = deployment.DistrictId;
        deploymentItem.FrontLineAmbulances = deployment.FrontLineAmbulances;
        deploymentItem.Name = deployment.Name;
        deploymentItem.OffRoadAmbulances = deployment.OffRoadAmbulances;
        deploymentItem.LastModified = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<DeploymentSummary> GetAllAsync(DateOnly startDate, DateOnly endDate)
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        await foreach (var deployment in context.Deployments
            .Where(d => d.Date >= startDate && d.Date <= endDate)
            .Select(d => new DeploymentSummary
            {
                AllWheelDriveAmbulances = d.AllWheelDriveAmbulances,
                Date = d.Date,
                District = d.District.Name,
                DistrictId = d.DistrictId,
                FrontLineAmbulances = d.FrontLineAmbulances,
                Name = d.Name,
                OffRoadAmbulances = d.OffRoadAmbulances,
                Region = d.District.Region,
            })
            .AsAsyncEnumerable())
        {
            yield return deployment;
        }
    }

    /// <inheritdoc/>
    public async Task<NationalSummary> GetNationalSummaryAsync()
    {
        var endDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date);
        var startDate = endDate.AddYears(-1);

        using var context = await contextBuilder.CreateDbContextAsync();

        return new NationalSummary
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
                        .Select(d => new DistrictSummary
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
    public async IAsyncEnumerable<PeakLoads> GetPeakLoadsAsync()
    {
        var endDate = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date);
        var startDate = endDate.AddYears(-1);

        using var context = await contextBuilder.CreateDbContextAsync();

        await foreach (var c in context.Deployments.Where(d => d.Date >= startDate && d.Date <= endDate)
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
            .AsAsyncEnumerable())
        {
            yield return c;
        }
    }

    private static Func<IGrouping<int, Deployment>, Dictionary<DateOnly, int>> CountVehicles(Func<Deployment, int> selector) =>
        d => d.GroupBy(d => d.Date)
          .Select(d => new { Date = d.Key, Count = d.Sum(selector) })
          .Where(d => d.Count > 0)
          .ToDictionary(d => d.Date, d => d.Count);
}
