// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SJAData.Client.Model;
using SJAData.Client.Model.Hours;
using SJAData.Data;
using SJAData.Model.Hours;
using SJAData.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SJAData.Services;

/// <summary>
/// Service for managing hours entries.
/// </summary>
/// <param name="dataContextFactory">The data context containing the hours data.</param>
/// <param name="logger">The logger to write to.</param>
public partial class HoursService(TimeProvider timeProvider, IDbContextFactory<ApplicationDbContext> dataContextFactory, ILogger<HoursService> logger) : ILocalHoursService
{
    private readonly IDbContextFactory<ApplicationDbContext> dataContextFactory = dataContextFactory;
    private readonly ILogger<HoursService> logger = logger;
    private readonly TimeProvider timeProvider = timeProvider;

    /// <inheritdoc/>
    public async Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours, string userId)
    {
        var dataContext = await dataContextFactory.CreateDbContextAsync();

        var existingPeople = await dataContext.People.ToDictionaryAsync(s => s.Id, s => s);

        var hoursList = await hours.ToListAsync();

        var startDate = hoursList.Min(h => h.ShiftDate);
        var endDate = hoursList.Max(h => h.ShiftDate);

        var existingItems = await dataContext.Hours.Where(h => h.Date >= startDate && h.Date <= endDate)
            .ToListAsync();

        foreach (var h in hoursList.Where(h => !string.IsNullOrWhiteSpace(h.IdNumber)).OrderBy(h => h.ShiftDate).ThenBy(d => d.ShiftLength))
        {
            var idValid = int.TryParse(h.IdNumber, out var id);

            if (!idValid)
            {
                continue;
            }

            if (!existingPeople.TryGetValue(id, out var person))
            {
                var nameParts = h.Name.Split(' ', 2);
                person = new Person { Id = id, FirstName = nameParts[1], LastName = nameParts[0], Region = Region.Undefined, UpdatedById = userId };
                dataContext.Add(person);
                existingPeople.Add(id, person);
            }

            var existingItem = existingItems.Find(i => i.PersonId == id && i.Date == h.ShiftDate);

            if (existingItem is null)
            {
                existingItem = new HoursEntry();
                existingItems.Add(existingItem);
                dataContext.Hours.Add(existingItem);

                existingItem.PersonId = id;
                existingItem.Date = h.ShiftDate;
                existingItem.DeletedAt = null;
                existingItem.UpdatedById = userId;
            }

            var trust = CalculateTrust(h);

            if (existingItem.Trust != trust)
            {
                existingItem.Trust = trust;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
                existingItem.UpdatedById = userId;
            }

            if (Math.Abs(existingItem.Hours - h.ShiftLength.TotalHours) > 0.1)
            {
                existingItem.Hours = h.ShiftLength.TotalHours;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
                existingItem.UpdatedById = userId;
            }

            var region = h.CrewType.Equals("Event Cover Amb", StringComparison.InvariantCultureIgnoreCase) ? person.Region : Region.Undefined;

            if (existingItem.Region != region)
            {
                existingItem.Region = region;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
                existingItem.UpdatedById = userId;
            }
        }

        foreach (var h in existingItems)
        {
            if (!hoursList.Exists(i => i.ShiftDate == h.Date && i.IdNumber == h.PersonId.ToString()))
            {
                h.UpdatedAt = DateTimeOffset.UtcNow;
                h.DeletedAt = h.UpdatedAt;
                h.UpdatedById = userId;
            }
        }

        return await dataContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<HoursCount> CountAsync(DateOnly? date, DateType? dateType = DateType.Month, bool future = false)
    {
        var dataContext = await dataContextFactory.CreateDbContextAsync();

        var items = dataContext.Hours.AsNoTracking()
            .Where(i => i.DeletedAt == null && i.Person.IsVolunteer && (i.Region != Data.Region.Undefined || i.Trust != Trust.Undefined));

        if (!date.HasValue)
        {
            date = DateOnly.FromDateTime(DateTime.Today);
            dateType = DateType.Year;
        }

        items = dateType switch
        {
            DateType.Day => items.Where(h => h.Date == date.Value),
            DateType.Month => items.Where(h => h.Date.Month == date.Value.Month && h.Date.Year == date.Value.Year),
            _ => items.Where(h => h.Date.Year == date.Value.Year),
        };

        if (future)
        {
            items = items.Where(d => d.Date > date);
        }
        else
        {
            items = items.Where(d => d.Date <= date);
        }

        var hoursCount = (await items.Select(h => new
        {
            h.Region,
            h.Trust,
            h.Hours,
        }).ToListAsync()).Select(h => new
        {
            Label = h.Trust == Trust.Undefined ? h.Region.ToString() : h.Trust.ToString(),
            h.Hours,
        })
        .Where(c => !string.IsNullOrEmpty(c.Label))
        .GroupBy(h => h.Label)
        .ToDictionary(h => h.Key, h => TimeSpan.FromHours(h.Sum(i => i.Hours)));
        var lastUpdate = await GetLastModifiedAsync();

        var result = new HoursCount
        {
            Counts = [],
            LastUpdate = lastUpdate,
        };

        foreach (var kvp in hoursCount)
        {
            result.Counts.Add(kvp.Key, kvp.Value);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future)
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{dateType}-{date}-{future}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        var dataContext = await dataContextFactory.CreateDbContextAsync();

        if (await dataContext.Hours.AnyAsync())
        {
            return await dataContext.Hours.MaxAsync(p => p.DeletedAt ?? p.UpdatedAt);
        }

        return DateTimeOffset.MinValue;
    }

    /// <inheritdoc/>
    public Task<int> GetNhseTargetAsync()
    {
        return Task.FromResult(4000);
    }

    /// <inheritdoc/>
    public async Task<string> GetNhseTargetEtagAsync()
    {
        var lastModified = await GetNhseTargetLastModifiedAsync();
        var date = new DateOnly(timeProvider.GetLocalNow().Year, timeProvider.GetLocalNow().Month, 1);
        var marker = $"{date}-{lastModified}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(marker));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    public Task<DateTimeOffset> GetNhseTargetLastModifiedAsync()
    {
        return Task.FromResult(DateTimeOffset.UtcNow);
    }

    private static Trust CalculateTrust(HoursFileLine hour) => hour.CrewType switch
    {
        "NHS E EEAST" => Trust.EastOfEnglandAmbulanceService,
        "NHS E EMAS" => Trust.EastMidlandsAmbulanceService,
        "NHS E IOW" => Trust.IsleOfWightAmbulanceService,
        "NHS E LAS" => Trust.LondonAmbulanceService,
        "NHS E NEAS" => Trust.NorthEastAmbulanceService,
        "NHS E NWAS" => Trust.NorthWestAmbulanceService,
        "NWAS 365" => Trust.NorthWestAmbulanceService,
        "NHS E SCAS" => Trust.SouthCentralAmbulanceService,
        "NHS E SECAmb" => Trust.SouthEastCoastAmbulanceService,
        "NHS E SWAST" => Trust.SouthWesternAmbulanceService,
        "NHS E YAS" => Trust.YorkshireAmbulanceService,
        "YAS" => Trust.YorkshireAmbulanceService,
        _ => Trust.Undefined,
    };
}
