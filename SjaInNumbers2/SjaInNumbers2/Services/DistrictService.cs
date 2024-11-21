// <copyright file="DistrictService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers2.Client.Model;
using SjaInNumbers2.Client.Model.Districts;
using SjaInNumbers2.Client.Services.Interfaces;
using SjaInNumbers2.Data;

namespace SjaInNumbers2.Services;

/// <summary>
/// Service for managing districts.
/// </summary>
public class DistrictService(IDbContextFactory<ApplicationDbContext> contextBuilder) : IDistrictsService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextBuilder = contextBuilder;

    /// <inheritdoc/>
    public async Task<int?> GetIdByNameAsync(string name, Region region)
    {
        using var context = await contextBuilder.CreateDbContextAsync();
        name = name.Trim();

        return await context.Districts
            .Include(d => d.PreviousNames)
            .Where(d => d.Region == region && (d.Name == name || d.PreviousNames.Any(e => e.OldName == name)))
            .Select(s => s.Id)
            .Cast<int?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<int?> GetIdByDistrictCodeAsync(string code)
    {
        using var context = await contextBuilder.CreateDbContextAsync();
        return await context.Districts
            .Where(d => d.Code == code)
            .Select(s => s.Id)
            .Cast<int?>()
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<DistrictSummary> GetDistrictSummariesAsync()
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        await foreach (var d in context.Districts.Select(s => new DistrictSummary
        {
            Code = s.Code,
            Id = s.Id,
            Name = s.Name,
            Region = s.Region,
        }).AsAsyncEnumerable())
        {
            yield return d;
        }
    }

    /// <inheritdoc/>
    public async Task<DistrictSummary?> GetDistrictAsync(int id)
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        return await context.Districts
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
        using var context = await contextBuilder.CreateDbContextAsync();
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
        using var context = await contextBuilder.CreateDbContextAsync();
        return !await context.Districts.AnyAsync(d => d.Id != id && d.Code == code);
    }

    /// <inheritdoc/>
    public async Task<bool> SetDistrictNameAsync(int id, string name)
    {
        using var context = await contextBuilder.CreateDbContextAsync();
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
        using var context = await contextBuilder.CreateDbContextAsync();
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
