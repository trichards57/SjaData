// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.Converters;
using SjaData.Server.Model.Hours;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

/// <summary>
/// Service for managing hours entries.
/// </summary>
/// <param name="dataContext">The data context containing the hours data.</param>
/// <param name="logger">The logger to write to.</param>
public partial class HoursService(DataContext dataContext, ILogger<HoursService> logger) : IHoursService
{
    private readonly DataContext dataContext = dataContext;
    private readonly ILogger<HoursService> logger = logger;

    /// <inheritdoc/>
    public async Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours)
    {
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
                person = new Data.Person { Id = id, FirstName = nameParts[1], LastName = nameParts[0], Region = Region.Undefined };
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
            }

            var trust = CalculateTrust(h);

            if (existingItem.Trust != trust)
            {
                existingItem.Trust = trust;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            }

            if (Math.Abs(existingItem.Hours - h.ShiftLength.TotalHours) < 0.1)
            {
                existingItem.Hours = h.ShiftLength.TotalHours;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            }

            var region = h.CrewType.Equals("Event Cover Amb", StringComparison.InvariantCultureIgnoreCase) ? person.Region : Region.Undefined;

            if (existingItem.Region != region)
            {
                existingItem.Region = region;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        foreach (var h in existingItems)
        {
            if (!hoursList.Exists(i => i.ShiftDate == h.Date && i.IdNumber == h.PersonId.ToString()))
            {
                h.UpdatedAt = DateTimeOffset.UtcNow;
                h.DeletedAt = h.UpdatedAt;
            }
        }

        return await dataContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<HoursCount> CountAsync(DateOnly? date, DateType? dateType, bool future)
    {
        var items = dataContext.Hours
            .Where(i => i.DeletedAt == null && i.Person.IsVolunteer && (i.Region != Region.Undefined || i.Trust != Trust.Undefined));

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
            items = items.Where(d => d.Date <= DateOnly.FromDateTime(DateTime.Today));
        }

        var hoursCount = (await items.Select(h => new
        {
            h.Region,
            h.Trust,
            h.Hours,
        }).ToListAsync()).Select(h => new
        {
            Label = h.Trust == Trust.Undefined ? RegionConverter.ToString(h.Region) : TrustConverter.ToString(h.Trust),
            h.Hours,
        })
        .Where(c => !string.IsNullOrEmpty(c.Label))
        .GroupBy(h => h.Label)
        .ToDictionary(h => h.Key, h => TimeSpan.FromHours(h.Sum(i => i.Hours)));
        var lastUpdate = await GetLastModifiedAsync();

        return new HoursCount { Counts = new AreaDictionary<TimeSpan>(hoursCount), LastUpdate = lastUpdate };
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var existingItem = await dataContext.Hours.FirstOrDefaultAsync(h => h.Id == id && !h.DeletedAt.HasValue);

        if (existingItem is not null)
        {
            existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            existingItem.DeletedAt = existingItem.UpdatedAt;
            await dataContext.SaveChangesAsync();
            LogItemDeleted(id);
        }
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        if (await dataContext.Hours.AnyAsync())
        {
            return await dataContext.Hours.MaxAsync(p => p.DeletedAt ?? p.UpdatedAt);
        }

        return DateTimeOffset.MinValue;
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

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "Hours entry {id} has been deleted.")]
    private partial void LogItemDeleted(int id);
}
