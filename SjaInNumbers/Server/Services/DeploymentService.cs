// <copyright file="DeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Server.Services.Interfaces;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// Service for managing deployments.
/// </summary>
public class DeploymentService(ApplicationDbContext context) : IDeploymentService
{
    private readonly ApplicationDbContext context = context;

    /// <inheritdoc/>
    public async Task AddDeploymentAsync(NewDeployment deployment)
    {
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
    public IAsyncEnumerable<DeploymentSummary> GetAllAsync(DateOnly startDate, DateOnly endDate)
    {
        return context.Deployments
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
            .AsAsyncEnumerable();
    }

    /// <inheritdoc/>
    public async Task<NationalSummary> GetNationalSummaryAsync(DateOnly startDate, DateOnly endDate)
    {
        return new NationalSummary
        {
            Regions = await context
                .Deployments
                .Include(d => d.District)
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
                            FrontLineAmbulances = d.GroupBy(d => d.Date).Select(d => new { Date = d.Key, Count = d.Sum(d => d.FrontLineAmbulances) }).ToDictionary(d => d.Date, d => d.Count),
                            AllWheelDriveAmbulances = d.GroupBy(d => d.Date).Select(d => new { Date = d.Key, Count = d.Sum(d => d.AllWheelDriveAmbulances) }).ToDictionary(d => d.Date, d => d.Count),
                            OffRoadAmbulances = d.GroupBy(d => d.Date).Select(d => new { Date = d.Key, Count = d.Sum(d => d.OffRoadAmbulances) }).ToDictionary(d => d.Date, d => d.Count),
                        })
                        .ToList(),
                })
                .ToDictionaryAsync(r => r.Region, r => r.Summaries),
        };
    }
}
