// <copyright file="DistrictService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// Service for managing districts.
/// </summary>
public class DistrictService(ApplicationDbContext context) : IDistrictService
{
    private readonly ApplicationDbContext context = context;

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
        var district = await context.Districts.FindAsync(id);

        if (district is null)
        {
            return false;
        }

        district.Code = code;

        await context.SaveChangesAsync();

        return true;
    }
}
