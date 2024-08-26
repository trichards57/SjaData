// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Model;
using SjaData.Model.Converters;
using SjaData.Model.Hours;
using SjaData.Server.Api.Model;
using SjaData.Server.Data;
using SjaData.Server.Logging;
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
    public async Task AddAsync(NewHoursEntry hours)
    {
        var existingItem = await dataContext.Hours.FirstOrDefaultAsync(h => h.PersonId == hours.PersonId && h.Date == hours.Date);
        var newItem = false;

        if (existingItem is null)
        {
            newItem = true;
            existingItem = new HoursEntry();
            dataContext.Hours.Add(existingItem);

            existingItem.PersonId = hours.PersonId;
            existingItem.Date = hours.Date;
            existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            existingItem.DeletedAt = null;
        }

        existingItem.Trust = hours.Trust;
        existingItem.Hours = hours.Hours;
        existingItem.Region = hours.Region;
        existingItem.Name = hours.Name;

        await dataContext.SaveChangesAsync();

        if (newItem)
        {
            LogItemCreated(hours);
        }
        else
        {
            LogItemModified(existingItem.Id, hours);
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

    /// <inheritdoc/>
    public async Task<HoursCount> CountAsync(HoursQuery query)
    {
        var items = dataContext.Hours.AsQueryable();

        if (query.Date.HasValue)
        {
            items = query.DateType switch
            {
                DateType.Day => items.Where(h => h.Date == query.Date.Value),
                DateType.Month => items.Where(h => h.Date.Month == query.Date.Value.Month && h.Date.Year == query.Date.Value.Year),
                _ => items.Where(h => h.Date.Year == query.Date.Value.Year),
            };
        }

        var hoursCount = (await items.Where(i => i.DeletedAt == null).Select(h => new
        {
            h.Region,
            h.Trust,
            h.Hours,
        }).ToListAsync())
        .Select(h => new
        {
            h.Region,
            Label = h.Region == Region.Undefined ? TrustConverter.ToString(h.Trust) : RegionConverter.ToString(h.Region),
            h.Hours,
        })
        .GroupBy(h => h.Label)
        .ToDictionary(h => h.Key, h => TimeSpan.FromHours(h.Sum(i => i.Hours.TotalHours)));
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

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "Hours entry {id} has been deleted.")]
    private partial void LogItemDeleted(int id);

    [LoggerMessage(EventCodes.ItemCreated, LogLevel.Information, "Hour entry has been created.")]
    private partial void LogItemCreated([LogProperties]NewHoursEntry hours);

    [LoggerMessage(EventCodes.ItemModified, LogLevel.Information, "Hour entry {id} has been updated.")]
    private partial void LogItemModified(int id, [LogProperties]NewHoursEntry hours);
}
