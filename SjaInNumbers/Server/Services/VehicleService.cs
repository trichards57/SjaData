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
/// <param name="contextFactory">The factory for a data context containing the hours data.</param>
public class VehicleService(IDbContextFactory<ApplicationDbContext> contextFactory) : IVehicleService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextFactory = contextFactory;
    private readonly string[] disposalMarkings = ["to be sold", "dispose", "disposal"];

    /// <inheritdoc/>
    public async Task AddEntriesAsync(IEnumerable<VorIncident> vorIncidents)
    {
        var incidents = vorIncidents.ToList();

        using var context = await contextFactory.CreateDbContextAsync();

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
                await AddSingleEntryAsync(context, i, updateVors);
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
        using var context = await contextFactory.CreateDbContextAsync();

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
            }))
        {
            yield return v;
        }
    }

    /// <inheritdoc/>
    public async Task<VehicleSettings?> GetSettingsAsync(int id)
    {
        using var context = await contextFactory.CreateDbContextAsync();

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

        using var context = await contextFactory.CreateDbContextAsync();

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
        using var context = await contextFactory.CreateDbContextAsync();

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

    private async Task AddSingleEntryAsync(ApplicationDbContext context, VorIncident vorIncident, bool updateVors)
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
        using var context = await contextFactory.CreateDbContextAsync();

        foreach (var v in context.Vehicles
           .GetActive()
           .GetForPlace(place)
           .Select(s => new VorStatus
           {
               CallSign = s.CallSign,
               DueBack = s.IsVor ? s.Incidents.OrderByDescending(i => i.StartDate).First().EstimatedEndDate : null,
               HubId = s.HubId,
               IsVor = s.IsVor,
               Registration = s.Registration,
               Summary = s.IsVor ? s.Incidents.OrderByDescending(i => i.StartDate).First().Description : null,
               Id = s.Id,
           }))
        {
            yield return v;
        }
    }
}
