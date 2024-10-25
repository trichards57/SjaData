// <copyright file="HubService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Server.Services;

public class HubService(ApplicationDbContext context) : IHubService
{
    private readonly ApplicationDbContext context = context;

    /// <inheritdoc/>
    public IAsyncEnumerable<HubSummary> GetAllAsync()
    {
        return context.Hubs.Select(s => new HubSummary
        {
            District = s.District.Name,
            Name = s.Name,
            Id = s.Id,
            Region = s.District.Region,
        }).AsAsyncEnumerable();
    }

    /// <inheritdoc/>
    public async Task<HubName?> GetNameAsync(int id)
    {
        return await context.Hubs.Where(s => s.Id == id).Select(s => new HubName
        {
            Name = s.Name,
        }).Cast<HubName?>().FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> SetNameAsync(int id, HubName hubName)
    {
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
}
