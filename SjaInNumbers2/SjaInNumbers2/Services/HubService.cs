// <copyright file="HubService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers2.Client.Model.Hubs;
using SjaInNumbers2.Client.Services.Interfaces;
using SjaInNumbers2.Data;
using System.Security.Cryptography;
using System.Text;

namespace SjaInNumbers2.Services;

/// <summary>
/// Service for managing hubs.
/// </summary>
public class HubService(IDbContextFactory<ApplicationDbContext> contextFactory) : IHubService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextBuilder = contextFactory;

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        return await context.Hubs.AnyAsync() ? await context.Hubs.MaxAsync(h => h.UpdatedAt) : DateTimeOffset.MinValue;
    }

    /// <inheritdoc/>
    public async Task<string> GetAllEtagAsync()
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(lastModified.ToString()));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<HubSummary> GetHubSummariesAsync()
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        await foreach (var h in context.Hubs.Select(s => new HubSummary
        {
            District = s.District.Name,
            Name = s.Name,
            Id = s.Id,
            Region = s.District.Region,
            VehicleCount = s.Vehicles.Count,
            PeopleCount = s.People.Count,
        }).AsAsyncEnumerable())
        {
            yield return h;
        }
    }

    /// <inheritdoc/>
    public async Task<HubName?> GetHubNameAsync(int id)
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        return await context.Hubs.Where(s => s.Id == id).Select(s => new HubName
        {
            Name = s.Name,
        }).Cast<HubName?>().FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> SetHubNameAsync(int id, HubName hubName)
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        var hub = await context.Hubs
            .AsNoTracking()
            .Where(h => h.Id == id).Select(h => new { h.Name })
            .FirstOrDefaultAsync();

        if (hub == null)
        {
            return false;
        }

        if (hub.Name != hubName.Name)
        {
            var update = new Hub
            {
                Id = id,
            };

            context.Attach(update);

            update.Name = hubName.Name;

            await context.SaveChangesAsync();
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<HubSummary> AddHubAsync(NewHub newHub)
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        var hub = new Hub
        {
            Name = newHub.Name,
            DistrictId = newHub.DistrictId,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        context.Hubs.Add(hub);
        await context.SaveChangesAsync();

        return await context.Hubs.Select(s => new HubSummary
        {
            District = s.District.Name,
            Name = s.Name,
            Id = s.Id,
            Region = s.District.Region,
        }).FirstOrDefaultAsync(h => h.Id == hub.Id);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteHubAsync(int id)
    {
        using var context = await contextBuilder.CreateDbContextAsync();

        var res = await context.Hubs.Where(h => h.Id == id).ExecuteDeleteAsync();

        return res > 0;
    }
}
