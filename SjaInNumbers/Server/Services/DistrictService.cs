// <copyright file="DistrictService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// Service for managing districts.
/// </summary>
public partial class DistrictService(ApplicationDbContext context, ILogger<DistrictService> logger) : IDistrictService
{
    private readonly ApplicationDbContext context = context;
    private readonly ILogger logger = logger;

    /// <inheritdoc/>
    public IAsyncEnumerable<DistrictSummary> GetAll()
    {
        LogRetrievedAllDistrictSummaries();

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
    public async Task<DistrictSummary> GetDistrict(int id)
    {
        var district = await context.Districts
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

        if (district == null)
        {
            LogDistrictNotFound(id);
            throw new ItemNotFoundException();
        }
        else
        {
            LogRetrievedDistrictSummary(id);
            return district.Value;
        }
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
    public async Task MergeDistrictsAsync(MergeDistrict mergeDistrict)
    {
        var sourceDistrict = await context.Districts.Include(d => d.Hubs).FirstOrDefaultAsync(d => d.Id == mergeDistrict.SourceDistrictId);
        var destinationDistrict = await context.Districts.Include(d => d.Hubs).FirstOrDefaultAsync(d => d.Id == mergeDistrict.DestinationDistrictId);

        if (sourceDistrict == null)
        {
            LogDistrictNotFound(mergeDistrict.SourceDistrictId);
            throw new ItemNotFoundException();
        }

        if (destinationDistrict == null)
        {
            LogDistrictNotFound(mergeDistrict.DestinationDistrictId);
            throw new ItemNotFoundException();
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

        LogMergedDistricts(mergeDistrict.SourceDistrictId, mergeDistrict.DestinationDistrictId);
    }

    /// <inheritdoc/>
    public async Task SetDistrictCodeAsync(int id, string code)
    {
        var district = new District
        {
            Id = id,
        };

        context.Districts.Attach(district);

        district.Code = code;
        district.LastModified = DateTimeOffset.UtcNow;

        var count = await context.SaveChangesAsync();

        if (count == 0)
        {
            LogDistrictNotFound(id);
            throw new ItemNotFoundException();
        }

        LogDistrictCodeUpdated(id, code);
    }

    /// <inheritdoc/>
    public async Task SetDistrictNameAsync(int id, string name)
    {
        var district = await context.Districts.Include(d => d.PreviousNames).FirstOrDefaultAsync(d => d.Id == id);

        name = name.Trim();

        if (district == null)
        {
            LogDistrictNotFound(id);
            throw new ItemNotFoundException();
        }

        if (district.Name.Equals(name))
        {
            return;
        }

        var oldName = district.Name;

        district.Name = name;

        if (!district.PreviousNames.Any(n => n.OldName == oldName))
        {
            district.PreviousNames.Add(new DistrictPreviousName { DistrictId = id, OldName = oldName });
        }

        district.LastModified = DateTimeOffset.UtcNow;

        await context.SaveChangesAsync();

        LogDistrictNameUpdated(id, name);
    }

    [LoggerMessage(1003, LogLevel.Information, "District code for {districtId} updated.")]
    private partial void LogDistrictCodeUpdated(int districtId, string newCode);

    [LoggerMessage(1004, LogLevel.Information, "District name for {districtId} updated.")]
    private partial void LogDistrictNameUpdated(int districtId, string name);

    [LoggerMessage(2001, LogLevel.Warning, "Could not find a district with the ID {districtId}.")]
    private partial void LogDistrictNotFound(int districtId);

    [LoggerMessage(1002, LogLevel.Information, "Retrieved all the district summaries.")]
    private partial void LogRetrievedAllDistrictSummaries();

    [LoggerMessage(1001, LogLevel.Information, "Retrieved the summary for district {districtId}.")]
    private partial void LogRetrievedDistrictSummary(int districtId);

    [LoggerMessage(1005, LogLevel.Information, "Merged district with the ID {sourceDistrictId} into district with the ID {destinationDistrictId}.")]
    private partial void LogMergedDistricts(int sourceDistrictId, int destinationDistrictId);
}
