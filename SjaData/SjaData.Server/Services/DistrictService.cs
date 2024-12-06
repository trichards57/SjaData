// <copyright file="DistrictService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Model;
using SjaData.Server.Model.Districts;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

/// <summary>
/// Service for managing districts.
/// </summary>
public class DistrictService(ApplicationDbContext context) : IDistrictService
{
    private readonly ApplicationDbContext context = context;

    /// <inheritdoc/>
    public Task<int?> GetIdByNameAsync(string name, Region region)
    {
        name = name.Trim();

        return context.Districts
            .Include(d => d.PreviousNames)
            .Where(d => d.Region == region && (d.Name == name || d.PreviousNames.Any(e => e.OldName == name)))
            .Select(s => s.Id)
            .Cast<int?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<int?> GetIdByDistrictCodeAsync(string code)
    {
        return context.Districts
            .Where(d => d.Code == code)
            .Select(s => s.Id)
            .Cast<int?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<DistrictSummary> GetAll()
    {
        return context.Districts
            .Select(s => new DistrictSummary
            {
                Code = s.Code,
                Id = s.Id,
                Name = s.Name,
                Region = s.Region,
            }).AsAsyncEnumerable();
    }

    /// <inheritdoc/>
    public Task<DistrictSummary?> GetDistrict(int id)
    {
        return context.Districts
            .Where(d => d.Id == id)
            .Select(s => new DistrictSummary
            {
                Code = s.Code,
                Id = s.Id,
                Name = s.Name,
                Region = s.Region,
            })
            .Cast<DistrictSummary?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> SetDistrictCodeAsync(int id, string code)
    {
        var district = new District
        {
            Id = id,
        };

        context.Districts.Attach(district);

        district.Code = code;
        district.LastModified = DateTimeOffset.UtcNow;

        var count = await context.SaveChangesAsync();

        return count == 1;
    }

    /// <inheritdoc/>
    public async Task<bool> CheckDistrictCodeAvailable(int id, string code)
    {
        return !await context.Districts.AnyAsync(d => d.Id != id && d.Code == code);
    }

    /// <inheritdoc/>
    public async Task<bool> SetDistrictNameAsync(int id, string name)
    {
        var district = await context.Districts.Include(d => d.PreviousNames).FirstOrDefaultAsync(d => d.Id == id);

        name = name.Trim();

        if (district == null)
        {
            return false;
        }

        if (district.Name.Equals(name))
        {
            return true;
        }

        var oldName = district.Name;

        district.Name = name;

        if (!district.PreviousNames.Any(n => n.OldName == oldName))
        {
            district.PreviousNames.Add(new DistrictPreviousName { DistrictId = id, OldName = oldName });
        }

        district.LastModified = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> MergeDistrictsAsync(MergeDistrict mergeDistrict)
    {
        var sourceDistrict = await context.Districts.Include(d => d.Hubs).FirstOrDefaultAsync(d => d.Id == mergeDistrict.SourceDistrictId);
        var destinationDistrict = await context.Districts.Include(d => d.Hubs).FirstOrDefaultAsync(d => d.Id == mergeDistrict.DestinationDistrictId);

        if (sourceDistrict == null || destinationDistrict == null)
        {
            return false;
        }

        foreach (var hub in sourceDistrict.Hubs)
        {
            hub.DistrictId = destinationDistrict.Id;
        }

        foreach (var name in sourceDistrict.PreviousNames)
        {
            name.DistrictId = destinationDistrict.Id;
        }

        destinationDistrict.PreviousNames.Add(new DistrictPreviousName { DistrictId = destinationDistrict.Id, OldName = sourceDistrict.Name });

        context.Districts.Remove(sourceDistrict);

        await context.SaveChangesAsync();

        return true;
    }
}
