// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;

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
    public async IAsyncEnumerable<VehicleSettings> GetSettingsAsync(Place place)
    {
        Console.WriteLine(place.CreateQuery());

        foreach (var v in context.Vehicles
            .GetNotDeleted()
            .GetForPlace(place)
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
            }))
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
            })
            .Cast<VehicleSettings?>()
            .FirstOrDefaultAsync();
    }

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
    public IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Place place)
    {
        if (!Enum.IsDefined(place.Region))
        {
            throw new ArgumentOutOfRangeException(nameof(place));
        }

        return GetVorStatusesPrivateAsync(place);
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

        vehicle.CallSign = settings.CallSign;
        vehicle.ForDisposal = settings.ForDisposal;
        vehicle.HubId = settings.HubId;
        vehicle.Registration = settings.Registration;
        vehicle.VehicleType = settings.VehicleType;
        vehicle.Deleted = null;
        vehicle.LastModified = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();
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

    private async IAsyncEnumerable<VorStatus> GetVorStatusesPrivateAsync(Place place)
    {
        await foreach (var v in context.Vehicles
            .Include(e => e.Incidents)
            .Include(e => e.Hub)
            .ThenInclude(e => e.District)
            .GetActive()
            .GetForPlace(place).AsAsyncEnumerable())
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
}
