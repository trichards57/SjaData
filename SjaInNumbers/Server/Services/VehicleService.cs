// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// A service for managing vehicles.
/// </summary>
/// <param name="context">The data context containing the hours data.</param>
public class VehicleService(ApplicationDbContext context) : IVehicleService
{
    private readonly ApplicationDbContext context = context;
    private readonly string[] disposalMarkings = ["to be sold", "dispose", "disposal"];

    /// <inheritdoc/>
    public async Task AddEntriesAsync(IEnumerable<VorIncident> vorIncidents)
    {
        var incidents = vorIncidents.ToList();

        using var scope = await context.Database.BeginTransactionAsync();

        var lastUpdate = await context.KeyDates.OrderBy(k => k.Id).FirstAsync();

        var fileDate = incidents.Max(i => i.UpdateDate);

        var updateVors = incidents.TrueForAll(i => i.UpdateDate == fileDate) && fileDate >= lastUpdate.LastUpdateFile;

        if (updateVors)
        {
            await context.Vehicles.GetNotDeleted().ExecuteUpdateAsync(u => u.SetProperty(v => v.IsVor, false));
        }

        try
        {
            foreach (var i in incidents)
            {
                await AddSingleEntryAsync(i, updateVors);
            }

            if (updateVors)
            {
                lastUpdate.LastUpdateFile = fileDate;
            }

            await scope.CommitAsync();
        }
        catch
        {
            await scope.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<FailureReport> GetFailureReports(DateOnly endDate, VehicleType type)
    {
        var vehicles = context.Vehicles
            .AsNoTracking()
            .GetActive()
            .Where(v => v.VehicleType == type)
            .Include(v => v.Incidents)
            .GroupBy(g => new { g.Make, g.Model, g.BodyType })
            .AsAsyncEnumerable();

        var startDate = endDate.AddDays(-365);

        await foreach (var group in vehicles)
        {
            var failureReports = group.Select(v => new VehicleFailureReport
            {
                AnnualFailures = v.Incidents.Count(i => i.StartDate >= startDate && i.EndDate <= endDate),
                AverageRepairTime = v.Incidents.Where(i => i.StartDate >= startDate && i.EndDate <= endDate).Select(i => EF.Functions.DateDiffDay(i.StartDate, i.EndDate)).Average(),
            });

            var (probability, stdDev) = GetFleetFailureProbability(failureReports);

            yield return new FailureReport
            {
                BodyType = group.Key.BodyType,
                FailureProbability = probability,
                FailureStdDev = stdDev,
                Make = group.Key.Make,
                Model = group.Key.Model,
                VehicleType = type,
            };
        }
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        var hubsLastModified = await context.Hubs.AnyAsync() ? await context.Hubs.Select(s => s.UpdatedAt).MaxAsync() : DateTimeOffset.MinValue;
        var vehiclesLastModified = await context.Vehicles.AnyAsync() ? await context.Vehicles.Select(s => s.LastModified).MaxAsync() : DateTimeOffset.MinValue;
        var incidentsLastModifiedDateOnly = await context.VehicleIncidents.AnyAsync() ? await context.VehicleIncidents.Select(s => s.EndDate).MaxAsync() : new DateOnly(2000, 1, 1);

        var incidentsLastModified = new DateTimeOffset(incidentsLastModifiedDateOnly.ToDateTime(new TimeOnly(16, 0, 0)), new TimeSpan(0));

        return new[] { incidentsLastModified, vehiclesLastModified, hubsLastModified }.Max();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<VehicleTypeStatus> GetNationalVorStatusesAsync()
    {
        var now = DateTime.Today;
        var thirteenMonthsAgo = now.AddMonths(-13);
        var twelveMonthsAgo = now.AddMonths(-12);
        var sevenMonthsAgo = now.AddMonths(-7);
        var sixMonthsAgo = now.AddMonths(-6);
        var fourMonthsAgo = now.AddMonths(-4);
        var threeMonthsAgo = now.AddMonths(-3);
        var oneMonthsAgo = now.AddMonths(-1);

        var thirteenMonthDays = (oneMonthsAgo - thirteenMonthsAgo).TotalDays;
        var twelveMonthDays = (now - twelveMonthsAgo).TotalDays;
        var sevenMonthDays = (oneMonthsAgo - sevenMonthsAgo).TotalDays;
        var sixMonthDays = (now - sixMonthsAgo).TotalDays;
        var fourMonthDays = (oneMonthsAgo - fourMonthsAgo).TotalDays;
        var threeMonthDays = (now - threeMonthsAgo).TotalDays;

        await foreach (var make in context.Vehicles
            .Include(e => e.Incidents)
            .GetActive()
            .GroupBy(e => e.Make.ToUpper()).AsAsyncEnumerable())
        {
            foreach (var model in make.GroupBy(v => v.Model.ToUpper()))
            {
                foreach (var bodyType in model.GroupBy(b => b.BodyType.ToUpper()))
                {
                    var incidents = bodyType.Select(v => v.Incidents);

                    yield return new VehicleTypeStatus
                    {
                        Make = bodyType.First().Make,
                        Model = bodyType.First().Model,
                        BodyType = bodyType.First().BodyType,
                        Total = bodyType.Count(),
                        CurrentlyAvailable = bodyType.Count(v => !v.IsVor),
                        AverageTwelveMonthMinusOneAvailability = incidents.Select(i => GetVorDates(i, DateOnly.FromDateTime(thirteenMonthsAgo), DateOnly.FromDateTime(oneMonthsAgo)).Count).Average() / thirteenMonthDays,
                        AverageTwelveMonthAvailability = incidents.Select(i => GetVorDates(i, DateOnly.FromDateTime(twelveMonthsAgo), DateOnly.FromDateTime(now)).Count).Average() / twelveMonthDays,
                        AverageSixMonthMinusOneAvailability = incidents.Select(i => GetVorDates(i, DateOnly.FromDateTime(sevenMonthsAgo), DateOnly.FromDateTime(oneMonthsAgo)).Count).Average() / sevenMonthDays,
                        AverageSixMonthAvailability = incidents.Select(i => GetVorDates(i, DateOnly.FromDateTime(sixMonthsAgo), DateOnly.FromDateTime(now)).Count).Average() / sixMonthDays,
                        AverageThreeMonthMinusOneAvailability = incidents.Select(i => GetVorDates(i, DateOnly.FromDateTime(fourMonthsAgo), DateOnly.FromDateTime(oneMonthsAgo)).Count).Average() / fourMonthDays,
                        AverageThreeMonthAvailability = incidents.Select(i => GetVorDates(i, DateOnly.FromDateTime(threeMonthsAgo), DateOnly.FromDateTime(now)).Count).Average() / threeMonthDays,
                    };
                }
            }
        }
    }

    /// <inheritdoc/>
    public Task<StringSegment> GetNationalVorStatusesEtagAsync() => GetLastModifiedEtagAsync();

    /// <inheritdoc/>
    public async IAsyncEnumerable<VehicleSettings> GetSettingsAsync()
    {
        await foreach (var v in context.Vehicles
            .GetNotDeleted()
            .AsNoTracking()
            .Select(s => new VehicleSettings
            {
                CallSign = s.CallSign,
                ForDisposal = s.ForDisposal,
                HubId = s.HubId,
                Id = s.Id,
                Registration = s.Registration,
                VehicleType = s.VehicleType,
                District = s.Hub == null ? "Unknown" : s.Hub.District.Name,
                Region = s.Hub == null ? Region.Undefined : s.Hub.District.Region,
                Hub = s.Hub == null ? "Unknown" : s.Hub.Name,
                Model = s.Model,
                Make = s.Make,
                BodyType = s.BodyType,
            }).AsAsyncEnumerable())
        {
            yield return v;
        }
    }

    /// <inheritdoc/>
    public async Task<VehicleSettings?> GetSettingsAsync(int id)
    {
        return await context.Vehicles
            .GetNotDeleted()
            .Where(v => v.Id == id)
            .Select(s => new VehicleSettings
            {
                CallSign = s.CallSign,
                ForDisposal = s.ForDisposal,
                HubId = s.HubId,
                Id = s.Id,
                Registration = s.Registration,
                VehicleType = s.VehicleType,
                District = s.Hub == null ? "Unknown" : s.Hub.District.Name,
                Region = s.Hub == null ? Region.Undefined : s.Hub.District.Region,
                Hub = s.Hub == null ? "Unknown" : s.Hub.Name,
                Model = s.Model,
                Make = s.Make,
                BodyType = s.BodyType,
            })
            .Cast<VehicleSettings?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<StringSegment> GetSettingsEtagAsync() => GetLastModifiedEtagAsync();

    /// <inheritdoc/>
    public async Task<StringSegment> GetSettingsEtagAsync(int id)
    {
        var lastModified = await context.Vehicles
            .Where(e => e.Id == id)
            .Select(s => s.LastModified)
            .FirstOrDefaultAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{id}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public async Task<NationalVehicleReport> GetVehicleReportAsync()
    {
        return await context.Vehicles
            .GetActive()
            .GroupBy(v => 1) // Group by a constant to get a single overall grouping
            .Select(g => new NationalVehicleReport
            {
                AllWheelDriveAmbulances = g.Count(v => v.VehicleType == VehicleType.AllWheelDrive),
                OffRoadAmbulances = g.Count(v => v.VehicleType == VehicleType.OffRoadAmbulance),
                FrontLineAmbulances = g.Count(v => v.VehicleType == VehicleType.FrontLineAmbulance),
                Districts = g
                    .Where(v => v.HubId.HasValue)
                    .GroupBy(v => v.Hub!.District)
                    .Select(dg => new DistrictVehicleReport
                    {
                        District = dg.Key.Name,
                        AllWheelDriveAmbulances = dg.Count(v => v.VehicleType == VehicleType.AllWheelDrive),
                        OffRoadAmbulances = dg.Count(v => v.VehicleType == VehicleType.OffRoadAmbulance),
                        FrontLineAmbulances = dg.Count(v => v.VehicleType == VehicleType.FrontLineAmbulance),
                        Region = dg.Key.Region,
                        DistrictId = dg.Key.Id,
                    })
                    .ToList(),
            })
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<StringSegment> GetVehicleReportEtagAsync() => GetLastModifiedEtagAsync();

    /// <inheritdoc/>
    public async Task<VorStatistics?> GetVorStatisticsAsync(Place place)
    {
        if (!Enum.IsDefined(place.Region))
        {
            throw new ArgumentOutOfRangeException(nameof(place));
        }

        var vehicles = await context.Vehicles
            .GetActive()
            .GetForPlace(place)
            .Select(v => new { v.IsVor })
            .AsNoTracking()
            .ToListAsync();

        var totalVehicles = vehicles.Count;
        var vorVehicles = vehicles.Count(v => v.IsVor);
        var availableVehicles = totalVehicles - vorVehicles;

        var endDate = DateTime.Now.Date;
        var startDate = endDate.AddYears(-1);

        var incidentsLastYear = await context.VehicleIncidents
            .GetForPlace(place)
            .Where(i => i.StartDate <= DateOnly.FromDateTime(endDate) && i.EndDate >= DateOnly.FromDateTime(startDate))
            .Select(i => new { i.StartDate, i.EndDate })
            .AsNoTracking()
            .ToListAsync();

        var dateRange = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                              .Select(offset => startDate.AddDays(offset))
                              .ToList();

        var incidentCountPerDay = new Dictionary<DateOnly, int>();

        foreach (var date in dateRange)
        {
            var d = DateOnly.FromDateTime(date);
            incidentCountPerDay[d] = totalVehicles - incidentsLastYear.Count(i => i.StartDate <= d && i.EndDate >= d);
        }

        var incidentsByMonth = incidentCountPerDay.GroupBy(i => new { i.Key.Year, i.Key.Month })
            .Select(g => new { Month = g.Key, Count = (int)Math.Round(g.Average(i => i.Value)) })
            .ToDictionary(i => new DateOnly(i.Month.Year, i.Month.Month, 1), i => i.Count);

        return new VorStatistics
        {
            AvailableVehicles = availableVehicles,
            TotalVehicles = totalVehicles,
            VorVehicles = vorVehicles,
            PastAvailability = incidentsByMonth,
        };
    }

    /// <inheritdoc/>
    public async Task<StringSegment> GetVorStatisticsEtagAsync(Place place)
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{place.Region}-{place.DistrictId}-{place.HubId}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Region region)
    {
        if (!Enum.IsDefined(region))
        {
            throw new InvalidEnumArgumentException(nameof(region), (int)region, typeof(Region));
        }

        return GetVorStatusesPrivateAsync(region);
    }

    /// <inheritdoc/>
    public async Task<StringSegment> GetVorStatusesEtagAsync(Region region)
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{region}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public async Task PutSettingsAsync(UpdateVehicleSettings settings)
    {
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(s => s.Registration == settings.Registration);

        if (vehicle == null)
        {
            vehicle = new Vehicle();
            context.Vehicles.Add(vehicle);
        }

        vehicle.CallSign = settings.CallSign.Trim().Replace(" ", string.Empty).ToUpper();
        vehicle.ForDisposal = settings.ForDisposal;
        vehicle.HubId = settings.HubId;
        vehicle.Registration = settings.Registration.Trim().Replace(" ", string.Empty).ToUpper();
        vehicle.VehicleType = settings.VehicleType;
        vehicle.Deleted = null;
        vehicle.Make = settings.Make.Trim().ToUpper();
        vehicle.Model = settings.Model.Trim().ToUpper();
        vehicle.BodyType = settings.BodyType.Trim();
        vehicle.LastModified = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();
    }

    private static (double Probability, double StdDev) GetFleetFailureProbability(IEnumerable<VehicleFailureReport> vehicles)
    {
        double combinedProbability = 1.0;
        double combinedVariance = 0.0;

        foreach (var vehicle in vehicles)
        {
            double vehicleFailureProbability = vehicle.GetDailyFailureProbability();
            double vehicleFailureVariance = vehicle.GetDailyFailureVariance();

            // Multiply the complements of probabilities for combined probability
            combinedProbability *= 1 - vehicleFailureProbability;

            // Aggregate variance (assuming independence)
            combinedVariance += vehicleFailureVariance;
        }

        // Fleet failure probability and standard deviation
        double fleetFailureProbability = 1 - combinedProbability;
        double fleetFailureStdDev = Math.Sqrt(combinedVariance);

        return (fleetFailureProbability, fleetFailureStdDev);
    }

    private static List<DateOnly> GetVorDates(IEnumerable<VehicleIncident> incidents, DateOnly start, DateOnly end)
    {
        var vorDates = new HashSet<DateOnly>();

        foreach (var i in incidents)
        {
            var s = i.StartDate < start ? start : i.StartDate;
            var e = i.EndDate > end ? end : i.EndDate;

            for (var date = s; date <= e; date = date.AddDays(1))
            {
                vorDates.Add(date);
            }
        }

        return [.. vorDates];
    }

    private async Task AddSingleEntryAsync(VorIncident vorIncident, bool updateVors)
    {
        var trimmedReg = vorIncident.Registration.ToUpperInvariant().Trim().Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
        var vehicle = await context.Vehicles.FirstOrDefaultAsync(v => v.Registration == trimmedReg);

        if (vehicle == null)
        {
            vehicle = new Vehicle
            {
                CallSign = vorIncident.CallSign.ToUpperInvariant().Trim().Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase),
                Registration = trimmedReg,
                BodyType = vorIncident.BodyType,
                Make = vorIncident.Make,
                Model = vorIncident.Model,
                LastModified = DateTimeOffset.UtcNow,
            };
            context.Vehicles.Add(vehicle);
        }
        else if (vehicle.Deleted != null)
        {
            vehicle.Deleted = null;
        }

        await context.SaveChangesAsync();

        var incident = await context.VehicleIncidents.FirstOrDefaultAsync(i => i.VehicleId == vehicle.Id && i.StartDate == vorIncident.StartDate);

        if (incident == null)
        {
            incident = new VehicleIncident
            {
                VehicleId = vehicle.Id,
                StartDate = vorIncident.StartDate,
                Description = vorIncident.Description,
            };
            context.VehicleIncidents.Add(incident);
        }

        if (incident.EndDate < vorIncident.UpdateDate)
        {
            incident.EndDate = vorIncident.UpdateDate;
            incident.Description = vorIncident.Description;
            incident.Comments = vorIncident.Comments;
            incident.EstimatedEndDate = vorIncident.EstimatedRepairDate;
        }

        if (updateVors)
        {
            vehicle.IsVor = true;
        }

        if (Array.Exists(disposalMarkings, s => vorIncident.Comments.Contains(s, StringComparison.OrdinalIgnoreCase) || vorIncident.Description.Contains(s, StringComparison.OrdinalIgnoreCase)))
        {
            vehicle.ForDisposal = true;
        }

        vehicle.LastModified = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync();
    }

    private async Task<StringSegment> GetLastModifiedEtagAsync()
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    private async IAsyncEnumerable<VorStatus> GetVorStatusesPrivateAsync(Region region)
    {
        await foreach (var v in context.Vehicles
            .Include(e => e.Incidents)
            .Include(e => e.Hub)
            .ThenInclude(e => e.District)
            .GetActive()
            .GetForPlace(new Place { Region = region }).AsAsyncEnumerable())
        {
            var now = DateTime.Today;
            var thirteenMonthsAgo = now.AddMonths(-13);
            var twelveMonthsAgo = now.AddMonths(-12);
            var sevenMonthsAgo = now.AddMonths(-7);
            var sixMonthsAgo = now.AddMonths(-6);
            var fourMonthsAgo = now.AddMonths(-4);
            var threeMonthsAgo = now.AddMonths(-3);
            var oneMonthsAgo = now.AddMonths(-1);

            var thirteenMonthDays = (oneMonthsAgo - thirteenMonthsAgo).TotalDays;
            var twelveMonthDays = (now - twelveMonthsAgo).TotalDays;
            var sevenMonthDays = (oneMonthsAgo - sevenMonthsAgo).TotalDays;
            var sixMonthDays = (now - sixMonthsAgo).TotalDays;
            var fourMonthDays = (oneMonthsAgo - fourMonthsAgo).TotalDays;
            var threeMonthDays = (now - threeMonthsAgo).TotalDays;

            yield return new VorStatus
            {
                CallSign = v.CallSign,
                DueBack = v.IsVor ? v.Incidents.OrderByDescending(i => i.StartDate).First().EstimatedEndDate : null,
                HubId = v.HubId,
                IsVor = v.IsVor,
                Registration = v.Registration,
                Summary = v.IsVor ? v.Incidents.OrderByDescending(i => i.StartDate).First().Description : null,
                Id = v.Id,
                District = v.Hub == null ? "Unknown" : v.Hub.District.Name,
                Hub = v.Hub == null ? "Unknown" : v.Hub.Name,
                Region = v.Hub == null ? Region.Undefined : v.Hub.District.Region,
                TwelveMonthMinusOneVorCount = GetVorDates(v.Incidents, DateOnly.FromDateTime(thirteenMonthsAgo), DateOnly.FromDateTime(oneMonthsAgo)).Count / thirteenMonthDays,
                TwelveMonthVorCount = GetVorDates(v.Incidents, DateOnly.FromDateTime(twelveMonthsAgo), DateOnly.FromDateTime(now)).Count / twelveMonthDays,
                SixMonthMinusOneVorCount = GetVorDates(v.Incidents, DateOnly.FromDateTime(sevenMonthsAgo), DateOnly.FromDateTime(oneMonthsAgo)).Count / sevenMonthDays,
                SixMonthVorCount = GetVorDates(v.Incidents, DateOnly.FromDateTime(sixMonthsAgo), DateOnly.FromDateTime(now)).Count / sixMonthDays,
                ThreeMonthMinusOneVorCount = GetVorDates(v.Incidents, DateOnly.FromDateTime(fourMonthsAgo), DateOnly.FromDateTime(oneMonthsAgo)).Count / fourMonthDays,
                ThreeMonthVorCount = GetVorDates(v.Incidents, DateOnly.FromDateTime(threeMonthsAgo), DateOnly.FromDateTime(now)).Count / threeMonthDays,
                VorDates = GetVorDates(v.Incidents, DateOnly.FromDateTime(thirteenMonthsAgo), DateOnly.FromDateTime(now)),
            };
        }
    }
}
