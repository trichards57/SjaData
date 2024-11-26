// <copyright file="DeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Azure;
using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Deployments;
using System.Security.Cryptography;
using System.Text;

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
    public async Task<NationalDeploymentSummary> GetNationalSummaryAsync(DateOnly startDate, DateOnly endDate, string etag)
    {
        var lastModified = await context.Deployments.Select(d => (DateTimeOffset?)d.LastModified)
            .Concat(context.Districts.Select(d => (DateTimeOffset?)d.LastModified))
            .Concat(context.Vehicles.Select(d => (DateTimeOffset?)d.LastModified))
            .MaxAsync() ?? DateTimeOffset.MinValue;
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{startDate}-{endDate}-{lastModified}"));
        var actualEtag = Convert.ToBase64String(hash);

        if (etag == $"W/\"{actualEtag}\"")
        {
            return new NationalDeploymentSummary
            {
                LastModified = lastModified,
                ETag = actualEtag,
            };
        }

        var summaries = await context.Deployments
            .Where(d => d.Date >= startDate && d.Date <= endDate)
            .GroupBy(d => new { d.District.Region, d.DistrictId, d.District.Name })
            .Select(g => new
            {
                g.Key.Region,
                DistrictSummary = new
                {
                    g.Key.DistrictId,
                    District = g.Key.Name,
                    g.Key.Region,
                    AmbulanceCounts = g.GroupBy(d => d.Date)
                        .Select(d => new
                        {
                            Date = d.Key,
                            FrontLineAmbulances = d.Sum(d => d.FrontLineAmbulances),
                            AllWheelDriveAmbulances = d.Sum(d => d.AllWheelDriveAmbulances),
                            OffRoadAmbulances = d.Sum(d => d.OffRoadAmbulances),
                        }),
                },
            })
            .GroupBy(g => g.Region)
            .ToDictionaryAsync(g => g.Key, g => g.Select(x => x.DistrictSummary).ToList());

        var regionSummaries = summaries
            .ToDictionary(r => r.Key, r => r.Value
                .Select(d => new DistrictDeploymentSummary
                {
                    AllWheelDriveAmbulances = d.AmbulanceCounts.ToDictionary(d => d.Date, d => d.AllWheelDriveAmbulances),
                    FrontLineAmbulances = d.AmbulanceCounts.ToDictionary(d => d.Date, d => d.FrontLineAmbulances),
                    OffRoadAmbulances = d.AmbulanceCounts.ToDictionary(d => d.Date, d => d.OffRoadAmbulances),
                    District = d.District,
                    DistrictId = d.DistrictId,
                }).ToList());

        return new NationalDeploymentSummary
        {
            Regions = regionSummaries,
            LastModified = lastModified,
            ETag = Convert.ToBase64String(hash),
        };
    }

    /// <inheritdoc/>
    public async Task<NationalPeakLoads> GetPeakLoadsAsync(DateOnly startDate, DateOnly endDate, string etag)
    {
        var lastModified = await context.Deployments.Select(d => (DateTimeOffset?)d.LastModified)
            .Concat(context.Districts.Select(d => (DateTimeOffset?)d.LastModified))
            .Concat(context.Vehicles.Select(d => (DateTimeOffset?)d.LastModified))
            .MaxAsync() ?? DateTimeOffset.MinValue;

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{startDate}-{endDate}-{lastModified}"));
        var actualEtag = Convert.ToBase64String(hash);

        if (etag == $"W/\"{actualEtag}\"")
        {
            return new NationalPeakLoads
            {
                LastModified = lastModified,
                ETag = actualEtag,
            };
        }

        return new NationalPeakLoads
        {
            Loads = context.Deployments.Where(d => d.Date >= startDate && d.Date <= endDate)
                    .GroupBy(d => d.District)
                    .Select(d => new PeakLoads
                    {
                        Region = d.Key.Region,
                        District = d.Key.Name,
                        DistrictId = d.Key.Id,
                        FrontLineAmbulances = d.Max(d => d.FrontLineAmbulances),
                        AllWheelDriveAmbulances = d.Max(d => d.AllWheelDriveAmbulances),
                        OffRoadAmbulances = d.Max(d => d.OffRoadAmbulances),
                    }),
            LastModified = lastModified,
            ETag = actualEtag,
        };
    }
}
