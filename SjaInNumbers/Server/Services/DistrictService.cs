// <copyright file="DistrictService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Shared.Model.Districts;
using SjaInNumbers.Server.Services.Interfaces;

namespace SjaInNumbers.Server.Services;

public class DistrictService(ApplicationDbContext context) : IDistrictService
{
    private readonly ApplicationDbContext context = context;

    public Task<int?> GetByDistrictCodeAsync(string code)
    {
        return context.Districts
            .Where(d => d.Code == code)
            .Select(s => s.Id)
            .Cast<int?>()
            .FirstOrDefaultAsync();
    }

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
}
