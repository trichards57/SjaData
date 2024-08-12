// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Model.Hours;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

public class HoursService(DataContext dataContext) : IHoursService
{
    private readonly DataContext dataContext = dataContext;

    public async Task AddAsync(NewHoursEntry hours)
    {
        var existingItem = await dataContext.Hours.FirstOrDefaultAsync(h => h.PersonId == hours.PersonId && h.Date == hours.Date);

        if (existingItem is null)
        {
            existingItem = new HoursEntry();
            dataContext.Add(existingItem);

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
    }

    public async Task<DateTimeOffset> GetLastModifiedAsync() => await dataContext.Hours.MaxAsync(p => p.DeletedAt ?? p.UpdatedAt);

    public async Task<HoursCount> CountAsync(HoursQuery query)
    {
        var items = dataContext.Hours.AsQueryable();

        if (query.Trust.HasValue && query.Trust != Trust.Undefined)
        {
            items = items.Where(h => h.Trust == query.Trust.Value);
        }

        if (query.Region.HasValue && query.Region != Region.Undefined)
        {
            items = items.Where(h => h.Region == query.Region.Value);
        }

        if (query.Date.HasValue)
        {
            items = query.DateType switch
            {
                Model.DateType.Day => items.Where(h => h.Date == query.Date.Value),
                Model.DateType.Month => items.Where(h => h.Date.Month == query.Date.Value.Month && h.Date.Year == query.Date.Value.Year),
                _ => items.Where(h => h.Date.Year == query.Date.Value.Year),
            };
        }

        var hoursCount = TimeSpan.FromHours(await items.Where(i => !i.DeletedAt.HasValue).SumAsync(h => h.Hours.TotalHours));
        var lastUpdate = await GetLastModifiedAsync();

        return new HoursCount { Count = hoursCount, LastUpdate = lastUpdate };
    }

    public async Task DeleteAsync(int id)
    {
        var existingItem = await dataContext.Hours.FirstOrDefaultAsync(h => h.Id == id && !h.DeletedAt.HasValue);

        if (existingItem is not null)
        {
            existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            existingItem.DeletedAt = existingItem.UpdatedAt;
            await dataContext.SaveChangesAsync();
        }
    }
}
