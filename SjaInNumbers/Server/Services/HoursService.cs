// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Helpers;
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
/// <param name="dataContextFactory">The factory for a data context containing the hours data.</param>
/// <param name="logger">The logger to write to.</param>
public partial class HoursService(TimeProvider timeProvider, IDbContextFactory<ApplicationDbContext> dataContextFactory, ILogger<HoursService> logger) : IHoursService
{
    private readonly IDbContextFactory<ApplicationDbContext> dataContextFactory = dataContextFactory;
    private readonly ILogger<HoursService> logger = logger;
    private readonly TimeProvider timeProvider = timeProvider;

    /// <inheritdoc/>
    public async Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours, string userId)
    {
        using var dataContext = await dataContextFactory.CreateDbContextAsync();

        var existingPeople = await dataContext.People.ToDictionaryAsync(s => s.Id, s => s);

        var hoursList = await hours.ToListAsync();

        var startDate = hoursList.Min(h => h.ShiftDate);
        var endDate = hoursList.Max(h => h.ShiftDate);

        var existingItems = await dataContext.Hours.Where(h => h.Date >= startDate && h.Date <= endDate)
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
                person = new Person { Id = id, FirstName = nameParts[1], LastName = nameParts[0], HubId = null, UpdatedById = userId };
                dataContext.Add(person);
                existingPeople.Add(id, person);
            }

            var existingItem = existingItems.Find(i => i.PersonId == id && i.Date == h.ShiftDate);

            if (existingItem is null)
            {
                existingItem = new HoursEntry();
                existingItems.Add(existingItem);
                dataContext.Hours.Add(existingItem);

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

            var region = (h.CrewType.Equals("Event Cover Amb", StringComparison.InvariantCultureIgnoreCase) && person.Hub != null) ? person.Hub.District.Region : Region.Undefined;

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

        return await dataContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<HoursCount> CountAsync(DateOnly? date, DateType? dateType = DateType.Month, bool future = false)
    {
        using var dataContext = await dataContextFactory.CreateDbContextAsync();

        var items = dataContext.Hours.AsNoTracking()
            .Where(i => i.DeletedAt == null && i.Person.IsVolunteer && (i.Region != Region.Undefined || i.Trust != Trust.Undefined));

        if (!date.HasValue)
        {
            date = DateOnly.FromDateTime(DateTime.Today);
            dateType = DateType.Year;
        }

        items = dateType switch
        {
            DateType.Day => items.Where(h => h.Date == date.Value),
            DateType.Month => items.Where(h => h.Date.Month == date.Value.Month && h.Date.Year == date.Value.Year),
            _ => items.Where(h => h.Date.Year == date.Value.Year),
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
        }).ToListAsync()).Select(h => new
        {
            Label = h.Trust == Trust.Undefined ? h.Region.ToString() : h.Trust.ToString(),
            h.Hours,
        })
        .Where(c => !string.IsNullOrEmpty(c.Label))
        .GroupBy(h => h.Label)
        .ToDictionary(h => h.Key, h => TimeSpan.FromHours(h.Sum(i => i.Hours)));
        var lastUpdate = await GetLastModifiedAsync();

        var result = new HoursCount
        {
            Counts = [],
            LastUpdate = lastUpdate,
        };

        foreach (var kvp in hoursCount)
        {
            result.Counts.Add(kvp.Key, kvp.Value);
        }

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future)
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{dateType}-{date}-{future}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        using var dataContext = await dataContextFactory.CreateDbContextAsync();

        if (await dataContext.Hours.AnyAsync())
        {
            return await dataContext.Hours.MaxAsync(p => p.DeletedAt ?? p.UpdatedAt);
        }

        return DateTimeOffset.MinValue;
    }

    /// <inheritdoc/>
    public Task<int> GetNhseTargetAsync() => Task.FromResult(4000);

    /// <inheritdoc/>
    public async Task<string> GetNhseTargetEtagAsync()
    {
        var lastModified = await GetNhseTargetLastModifiedAsync();
        var date = new DateOnly(timeProvider.GetLocalNow().Year, timeProvider.GetLocalNow().Month, 1);
        var marker = $"{date}-{lastModified}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(marker));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public Task<DateTimeOffset> GetNhseTargetLastModifiedAsync() => Task.FromResult(DateTimeOffset.UtcNow);

    /// <inheritdoc/>
    public async Task<Trends> GetTrendsAsync(Region region, bool nhse)
    {
        using var dataContext = await dataContextFactory.CreateDbContextAsync();

        var districts = await dataContext.People.Where(p => p.Hub != null && p.Hub.District.Region == region).Select(p => p.Hub.District).Distinct().ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        var reportDate = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

        IQueryable<HoursEntry> hours = dataContext.Hours.AsNoTracking();

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
        var regionAverages = await GetAverages(hours.Where(h => h.Person.Hub != null && h.Person.Hub.District.Region == region), reportDate);
        var regionAveragesMinusOne = await GetAverages(hours.Where(h => h.Person.Hub != null && h.Person.Hub.District.Region == region), reportDate.AddMonths(-1));

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
            { "region", await GetOverTime(hours.Where(h => h.Person.Hub != null && h.Person.Hub.District.Region == region), reportDate) },
        };

        foreach (var d in districts)
        {
            try
            {
                var districtAverages = await GetAverages(hours.Where(h => h.Person.Hub != null && h.Person.Hub.District == d), reportDate);
                var districtAverageMinusOne = await GetAverages(hours.Where(h => h.Person.Hub != null && h.Person.Hub.District == d), reportDate.AddMonths(-1));

                twelveMonthAverages[d.Name] = districtAverages.TwelveMonthAverage;
                twelveMonthMinusOneAverages[d.Name] = districtAverageMinusOne.TwelveMonthAverage;
                sixMonthAverages[d.Name] = districtAverages.SixMonthAverage;
                sixMonthMinusOneAverages[d.Name] = districtAverageMinusOne.SixMonthAverage;
                threeMonthAverages[d.Name] = districtAverages.ThreeMonthAverage;
                threeMonthMinusOneAverages[d.Name] = districtAverageMinusOne.ThreeMonthAverage;
                hoursResult[d.Name] = await GetOverTime(hours.Where(h => h.Person.Hub != null && h.Person.Hub.District == d), reportDate);
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
        };

        return trends;
    }

    /// <inheritdoc/>
    public async Task<string> GetTrendsEtagAsync(Region region, bool nhse)
    {
        ExceptionHelpers.ThrowIfUndefined(region);

        var lastModified = await GetLastModifiedAsync();

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date);
        var startDate = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

        var marker = $"{region}-{nhse}-{startDate}-{lastModified}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(marker));

        return $"\"{Convert.ToBase64String(hash)}\"";
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
