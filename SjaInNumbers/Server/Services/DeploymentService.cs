// <copyright file="DeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using AutoMapper;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// Service for managing deployments.
/// </summary>
public partial class DeploymentService(
    ApplicationDbContext context,
    IDistrictService districtService,
    ILogger<DeploymentService> logger,
    IMapper mapper,
    TimeProvider timeProvider) : IDeploymentService
{
    private readonly ApplicationDbContext context = context;
    private readonly Dictionary<string, int> districtCache = [];
    private readonly ILogger logger = logger;
    private readonly IMapper mapper = mapper;
    private readonly TimeProvider timeProvider = timeProvider;

    /// <inheritdoc/>
    public async Task<CountResponse> AddDeployments(IAsyncEnumerable<NewDeployment> deployments)
    {
        try
        {
            await foreach (var deployment in deployments)
            {
                if (deployment.DipsReference == 0)
                {
                    continue;
                }

                var districtId = await GetDistrictId(deployment.DistrictCode);

                if (districtId == 0)
                {
                    continue;
                }

                var deploymentItem = await context.Deployments.FirstOrDefaultAsync(d => d.DipsReference == deployment.DipsReference && d.Date == deployment.Date);

                if (deploymentItem == null)
                {
                    deploymentItem = new Deployment();
                    context.Deployments.Add(deploymentItem);
                }

                mapper.Map(deployment, deploymentItem);
                deploymentItem.DistrictId = districtId;
                deploymentItem.LastModified = DateTime.UtcNow;
            }

            var updatedCount = await context.SaveChangesAsync();

            LogAddedOrUpdatedDeployments(updatedCount);

            return new CountResponse { Count = updatedCount };
        }
        catch (CsvHelperException ex)
        {
            LogCouldNotProcessCsvData(ex);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<NationalSummary> GetNationalSummaryAsync()
    {
        var endDate = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var startDate = endDate.AddYears(-1);

        LogRequestedNationalSummary(startDate, endDate);

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
    public IAsyncEnumerable<PeakLoads> GetPeakLoadsAsync()
    {
        var endDate = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var startDate = endDate.AddYears(-1);

        LogRequestedPeakLoads(startDate, endDate);

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

    private async Task<int> GetDistrictId(string code)
    {
        code = code.ToUpper();

        if (districtCache.TryGetValue(code, out var id))
        {
            return id;
        }

        id = await districtService.GetIdByDistrictCodeAsync(code) ?? 0;
        districtCache[code] = id;
        return id;
    }

    [LoggerMessage(1001, LogLevel.Information, "Added or updated {numberOfDeployments} deployments.")]
    private partial void LogAddedOrUpdatedDeployments(int numberOfDeployments);

    [LoggerMessage(2001, LogLevel.Error, "Could not process the uploaded CSV data.")]
    private partial void LogCouldNotProcessCsvData(Exception exception);

    [LoggerMessage(1003, LogLevel.Information, "Requested the national summary from {startDate} to {endDate}.")]
    private partial void LogRequestedNationalSummary(DateOnly startDate, DateOnly endDate);

    [LoggerMessage(1002, LogLevel.Information, "Requested the peak loads from {startDate} to {endDate}.")]
    private partial void LogRequestedPeakLoads(DateOnly startDate, DateOnly endDate);
}
