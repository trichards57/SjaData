// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Model.Hours;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Hours;
using SjaInNumbers.Shared.Model.Trends;
using System.Security.Cryptography;
using System.Text;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// Service for managing hours entries.
/// </summary>
/// <param name="timeProvider">The provider for the current time.</param>
/// <param name="context">The data context containing the hours data.</param>
/// <param name="logger">The logger to write to.</param>
public partial class HoursService(TimeProvider timeProvider, ApplicationDbContext context, ILogger<HoursService> logger) : IHoursService
{
    private readonly ApplicationDbContext context = context;
    private readonly ILogger<HoursService> logger = logger;
    private readonly TimeProvider timeProvider = timeProvider;

    /// <inheritdoc/>
    public async Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours, string userId)
    {
        var existingPeople = await context.People.Include(d => d.District).ToDictionaryAsync(s => s.Id, s => s);

        var hoursList = await hours.ToListAsync();

        var startDate = hoursList.Min(h => h.ShiftDate);
        var endDate = hoursList.Max(h => h.ShiftDate);

        var existingItems = await context.Hours.Where(h => h.Date >= startDate && h.Date <= endDate)
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
                person = new Person { Id = id, FirstName = nameParts[1], LastName = nameParts[0], DistrictId = null, UpdatedById = userId };
                context.Add(person);
                existingPeople.Add(id, person);
            }

            var existingItem = existingItems.Find(i => i.PersonId == id && i.Date == h.ShiftDate);

            if (existingItem is null)
            {
                existingItem = new HoursEntry();
                existingItems.Add(existingItem);
                context.Hours.Add(existingItem);

                existingItem.PersonId = id;
                existingItem.Date = h.ShiftDate;
                existingItem.DeletedAt = null;
                existingItem.UpdatedById = userId;
            }

            var trust = CalculateTrust(h);

            if (existingItem.Trust != trust)
            {
                existingItem.Trust = trust;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
                existingItem.UpdatedById = userId;
            }

            if (Math.Abs(existingItem.Hours - h.ShiftLength.TotalHours) > 0.1)
            {
                existingItem.Hours = h.ShiftLength.TotalHours;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
                existingItem.UpdatedById = userId;
            }

            var region = (h.CrewType.Equals("Event Cover Amb", StringComparison.InvariantCultureIgnoreCase) && person.District != null) ? person.District.Region : Region.Undefined;

            if (existingItem.Region != region)
            {
                existingItem.Region = region;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
                existingItem.UpdatedById = userId;
            }
        }

        foreach (var h in existingItems)
        {
            if (!hoursList.Exists(i => i.ShiftDate == h.Date && i.IdNumber == h.PersonId.ToString()))
            {
                h.UpdatedAt = DateTimeOffset.UtcNow;
                h.DeletedAt = h.UpdatedAt;
                h.UpdatedById = userId;
            }
        }

        return await context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<HoursCount> CountAsync(DateOnly date, DateType dateType, bool future, string etag)
    {
        var lastModified = await context.Deployments.Select(d => (DateTimeOffset?)d.LastModified)
            .MaxAsync() ?? DateTimeOffset.MinValue;
        var actualEtag = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes($"{dateType}-{date}-{future}-{lastModified}")));

        if (etag == $"W/\"{actualEtag}\"")
        {
            return new HoursCount
            {
                LastModified = lastModified,
                ETag = actualEtag,
            };
        }

        var items = context.Hours.AsNoTracking()
            .Where(i => i.DeletedAt == null && i.Person.IsVolunteer && (i.Region != Region.Undefined || i.Trust != Trust.Undefined));

        items = dateType switch
        {
            DateType.Day => items.Where(h => h.Date == date),
            DateType.Month => items.Where(h => h.Date.Month == date.Month && h.Date.Year == date.Year),
            _ => items.Where(h => h.Date.Year == date.Year),
        };

        if (future)
        {
            items = items.Where(d => d.Date > date);
        }
        else
        {
            items = items.Where(d => d.Date <= date);
        }

        var hoursCount = (await items.Select(h => new
        {
            h.Region,
            h.Trust,
            h.Hours,
        }).ToListAsync())
            .Select(h => new
            {
                Label = h.Trust == Trust.Undefined ? h.Region.ToString() : h.Trust.ToString(),
                h.Hours,
            })
            .Where(c => !string.IsNullOrEmpty(c.Label))
            .GroupBy(h => h.Label)
            .ToDictionary(h => h.Key, h => TimeSpan.FromHours(h.Sum(i => i.Hours)));

        var result = new HoursCount
        {
            Counts = [],
            LastModified = lastModified,
            ETag = actualEtag,
        };

        foreach (var kvp in hoursCount)
        {
            result.Counts.Add(kvp.Key, kvp.Value);
        }

        return result;
    }

    /// <inheritdoc/>
    public Task<HoursTarget> GetNhseTargetAsync()
    {
        var lastModified = timeProvider.GetUtcNow();
        var date = new DateOnly(lastModified.Year, lastModified.Month, 1);
        var actualEtag = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes($"{date}-{lastModified}")));

        return Task.FromResult(new HoursTarget
        {
            Date = date,
            LastModified = lastModified,
            Target = 4000,
            ETag = actualEtag,
        });
    }

    /// <inheritdoc/>
    public async Task<Trends> GetTrendsAsync(Region region, bool nhse, string etag)
    {
        var lastModified = await context.Deployments.Select(d => (DateTimeOffset?)d.LastModified)
            .MaxAsync() ?? DateTimeOffset.MinValue;

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date);
        var reportDate = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

        var marker = $"{region}-{nhse}-{reportDate}-{lastModified}";
        var actualEtag = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(marker)));

        if (etag == $"W/\"{actualEtag}\"")
        {
            return new Trends
            {
                LastModified = lastModified,
                ETag = actualEtag,
            };
        }

        var districts = await context.People.Where(p => p.District != null && p.District.Region == region).Select(p => p.District).Distinct().ToListAsync();
        IQueryable<HoursEntry> hours = context.Hours.AsNoTracking();

        if (nhse)
        {
            hours = hours.Where(h => h.Trust != Trust.Undefined);
        }
        else
        {
            hours = hours.Where(h => h.Region != Region.Undefined || h.Trust != Trust.Undefined);
        }

        var nationalAverages = await GetAverages(hours, reportDate);
        var nationalAveragesMinusOne = await GetAverages(hours, reportDate.AddMonths(-1));
        var regionAverages = await GetAverages(hours.Where(h => h.Person.District != null && h.Person.District.Region == region), reportDate);
        var regionAveragesMinusOne = await GetAverages(hours.Where(h => h.Person.District != null && h.Person.District.Region == region), reportDate.AddMonths(-1));

        var twelveMonthAverages = new Dictionary<string, double>
        {
            { "national", nationalAverages.TwelveMonthAverage },
            { "region", regionAverages.TwelveMonthAverage },
        };
        var twelveMonthMinusOneAverages = new Dictionary<string, double>
        {
            { "national", nationalAveragesMinusOne.TwelveMonthAverage },
            { "region", regionAveragesMinusOne.TwelveMonthAverage },
        };
        var sixMonthAverages = new Dictionary<string, double>
        {
            { "national", nationalAverages.SixMonthAverage },
            { "region", regionAverages.SixMonthAverage },
        };
        var sixMonthMinusOneAverages = new Dictionary<string, double>
        {
            { "national", nationalAveragesMinusOne.SixMonthAverage },
            { "region", regionAveragesMinusOne.SixMonthAverage },
        };
        var threeMonthAverages = new Dictionary<string, double>
        {
            { "national", nationalAverages.ThreeMonthAverage },
            { "region", regionAverages.ThreeMonthAverage },
        };
        var threeMonthMinusOneAverages = new Dictionary<string, double>
        {
            { "national", nationalAveragesMinusOne.ThreeMonthAverage },
            { "region", regionAveragesMinusOne.ThreeMonthAverage },
        };
        var hoursResult = new Dictionary<string, double[]>
        {
            { "national", await GetOverTime(hours, reportDate) },
            { "region", await GetOverTime(hours.Where(h => h.Person.District != null && h.Person.District.Region == region), reportDate) },
        };

        foreach (var d in districts)
        {
            try
            {
                if (d == null)
                {
                    continue;
                }

                var districtAverages = await GetAverages(hours.Where(h => h.Person.District == d), reportDate);
                var districtAverageMinusOne = await GetAverages(hours.Where(h => h.Person.District == d), reportDate.AddMonths(-1));

                twelveMonthAverages[d.Name] = districtAverages.TwelveMonthAverage;
                twelveMonthMinusOneAverages[d.Name] = districtAverageMinusOne.TwelveMonthAverage;
                sixMonthAverages[d.Name] = districtAverages.SixMonthAverage;
                sixMonthMinusOneAverages[d.Name] = districtAverageMinusOne.SixMonthAverage;
                threeMonthAverages[d.Name] = districtAverages.ThreeMonthAverage;
                threeMonthMinusOneAverages[d.Name] = districtAverageMinusOne.ThreeMonthAverage;
                hoursResult[d.Name] = await GetOverTime(hours.Where(h => h.Person.District == d), reportDate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing district {District}", d);
            }
        }

        var trends = new Trends()
        {
            ReportDate = reportDate,
            TwelveMonthAverage = twelveMonthAverages,
            TwelveMonthMinusOneAverage = twelveMonthMinusOneAverages,
            SixMonthAverage = sixMonthAverages,
            SixMonthMinusOneAverage = sixMonthMinusOneAverages,
            ThreeMonthAverage = threeMonthAverages,
            ThreeMonthMinusOneAverage = threeMonthMinusOneAverages,
            Hours = hoursResult,
            LastModified = lastModified,
            ETag = actualEtag,
        };

        return trends;
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

    private static Task<Averages> GetAverages(IQueryable<HoursEntry> hours, DateOnly startDate)
    {
        var endDate = startDate.AddMonths(-12);

        return hours
            .Where(h => h.DeletedAt == null && h.Person.IsVolunteer)
            .Where(h => h.Date <= startDate && h.Date >= endDate)
            .GroupBy(h => 1)
            .Select(h => new Averages
            {
                TwelveMonthAverage = h.Select(s => s.Hours).Sum() / 12,
                SixMonthAverage = h.Where(i => i.Date >= startDate.AddMonths(-6)).Select(s => s.Hours).Sum() / 6,
                ThreeMonthAverage = h.Where(i => i.Date >= startDate.AddMonths(-3)).Select(s => s.Hours).Sum(s => s) / 3,
            }).FirstAsync();
    }

    private static async Task<double[]> GetOverTime(IQueryable<HoursEntry> hours, DateOnly startDate)
    {
        var endDate = startDate.AddMonths(-12);

        // Create an array to hold 12 months of data
        double[] monthlyHours = new double[12];

        // Fetch the relevant data, grouped by year and month
        var details = await hours
            .Where(h => h.DeletedAt == null && h.Person.IsVolunteer)
            .Where(h => h.Date <= startDate && h.Date >= endDate)
            .GroupBy(h => new { h.Date.Year, h.Date.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, HoursSum = g.Sum(h => h.Hours) })
            .ToListAsync();

        // Iterate over the details and map them to the correct months
        foreach (var detail in details)
        {
            var index = 11 - (((startDate.Year - detail.Year) * 12) + startDate.Month - detail.Month);
            if (index >= 0 && index < 12)
            {
                monthlyHours[index] = detail.HoursSum;
            }
        }

        return monthlyHours;
    }

    private readonly record struct Averages
    {
        public double TwelveMonthAverage { get; init; }

        public double SixMonthAverage { get; init; }

        public double ThreeMonthAverage { get; init; }
    }
}
