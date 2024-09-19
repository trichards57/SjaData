// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.Converters;
using SjaData.Server.Model.Hours;
using SjaData.Server.Services.Interfaces;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace SjaData.Server.Services;

/// <summary>
/// Service for managing hours entries.
/// </summary>
/// <param name="dataContext">The data context containing the hours data.</param>
/// <param name="logger">The logger to write to.</param>
public partial class HoursService(DataContext dataContext, ILogger<HoursService> logger) : IHoursService
{
    private readonly DataContext dataContext = dataContext;
    private readonly ILogger<HoursService> logger = logger;

    /// <inheritdoc/>
    public async Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours)
    {
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
                person = new Data.Person { Id = id, FirstName = nameParts[1], LastName = nameParts[0], Region = Region.Undefined };
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
            }

            var trust = CalculateTrust(h);

            if (existingItem.Trust != trust)
            {
                existingItem.Trust = trust;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            }

            if (Math.Abs(existingItem.Hours - h.ShiftLength.TotalHours) > 0.1)
            {
                existingItem.Hours = h.ShiftLength.TotalHours;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            }

            var region = h.CrewType.Equals("Event Cover Amb", StringComparison.InvariantCultureIgnoreCase) ? person.Region : Region.Undefined;

            if (existingItem.Region != region)
            {
                existingItem.Region = region;
                existingItem.DeletedAt = null;
                existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        foreach (var h in existingItems)
        {
            if (!hoursList.Exists(i => i.ShiftDate == h.Date && i.IdNumber == h.PersonId.ToString()))
            {
                h.UpdatedAt = DateTimeOffset.UtcNow;
                h.DeletedAt = h.UpdatedAt;
            }
        }

        return await dataContext.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<HoursCount> CountAsync(DateOnly? date, DateType? dateType, bool future)
    {
        var items = dataContext.Hours
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
            Label = h.Trust == Trust.Undefined ? RegionConverter.ToString(h.Region) : TrustConverter.ToString(h.Trust),
            h.Hours,
        })
        .Where(c => !string.IsNullOrEmpty(c.Label))
        .GroupBy(h => h.Label)
        .ToDictionary(h => h.Key, h => TimeSpan.FromHours(h.Sum(i => i.Hours)));
        var lastUpdate = await GetLastModifiedAsync();

        return new HoursCount { Counts = new AreaDictionary<TimeSpan>(hoursCount), LastUpdate = lastUpdate };
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var existingItem = await dataContext.Hours.FirstOrDefaultAsync(h => h.Id == id && !h.DeletedAt.HasValue);

        if (existingItem is not null)
        {
            existingItem.UpdatedAt = DateTimeOffset.UtcNow;
            existingItem.DeletedAt = existingItem.UpdatedAt;
            await dataContext.SaveChangesAsync();
            LogItemDeleted(id);
        }
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        if (await dataContext.Hours.AnyAsync())
        {
            return await dataContext.Hours.MaxAsync(p => p.DeletedAt ?? p.UpdatedAt);
        }

        return DateTimeOffset.MinValue;
    }

    public async Task<Trends> GetTrendsAsync(Region region, bool nhse)
    {
        var districts = await dataContext.People.Where(p => p.Region == region).Select(p => p.District).Distinct().ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        var startDate = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

        IQueryable<HoursEntry> hours = dataContext.Hours;

        if (nhse)
        {
            hours = hours.Where(h => h.Trust != Trust.Undefined);
        }
        else
        {
            hours = hours.Where(h => h.Region != Region.Undefined || h.Trust != Trust.Undefined);
        }

        var nationalAverages = await GetAverages(hours, startDate);
        var nationalAveragesMinusOne = await GetAverages(hours, startDate.AddMonths(-1));
        var regionAverages = await GetAverages(hours.Where(h => h.Person.Region == region), startDate);
        var regionAveragesMinusOne = await GetAverages(hours.Where(h => h.Person.Region == region), startDate.AddMonths(-1));

        var trends = new Trends() { ThresholdDate = startDate };
        trends.TwelveMonthAverage["national"] = nationalAverages.TwelveMonthAverage;
        trends.TwelveMonthAverage["region"] = regionAverages.TwelveMonthAverage;
        trends.TwelveMonthMinusOneAverage["national"] = nationalAveragesMinusOne.TwelveMonthAverage;
        trends.TwelveMonthMinusOneAverage["region"] = regionAveragesMinusOne.TwelveMonthAverage;
        trends.SixMonthAverage["national"] = nationalAverages.SixMonthAverage;
        trends.SixMonthAverage["region"] = regionAverages.SixMonthAverage;
        trends.SixMonthMinusOneAverage["national"] = nationalAveragesMinusOne.SixMonthAverage;
        trends.SixMonthMinusOneAverage["region"] = regionAveragesMinusOne.SixMonthAverage;
        trends.ThreeMonthAverage["national"] = nationalAverages.ThreeMonthAverage;
        trends.ThreeMonthAverage["region"] = regionAverages.ThreeMonthAverage;
        trends.ThreeMonthMinusOneAverage["national"] = nationalAveragesMinusOne.ThreeMonthAverage;
        trends.ThreeMonthMinusOneAverage["region"] = regionAveragesMinusOne.ThreeMonthAverage;
        trends.Hours["national"] = await GetOverTime(hours, startDate);
        trends.Hours["region"] = await GetOverTime(hours.Where(h => h.Person.Region == region), startDate);

        foreach (var d in districts)
        {
            try
            {
                var districtAverages = await GetAverages(hours.Where(h => h.Person.District == d), startDate);
                var districtAverageMinusOne = await GetAverages(hours.Where(h => h.Person.District == d), startDate.AddMonths(-1));

                trends.TwelveMonthAverage[d] = districtAverages.TwelveMonthAverage;
                trends.TwelveMonthMinusOneAverage[d] = districtAverageMinusOne.TwelveMonthAverage;
                trends.SixMonthAverage[d] = districtAverages.SixMonthAverage;
                trends.SixMonthMinusOneAverage[d] = districtAverageMinusOne.SixMonthAverage;
                trends.ThreeMonthAverage[d] = districtAverages.ThreeMonthAverage;
                trends.ThreeMonthMinusOneAverage[d] = districtAverageMinusOne.ThreeMonthAverage;
                trends.Hours[d] = await GetOverTime(hours.Where(h => h.Person.District == d), startDate);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing district {district}", d);
            }
        }

        return trends;
    }

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

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "Hours entry {id} has been deleted.")]
    private partial void LogItemDeleted(int id);

    /// <inheritdoc/>
    public async Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future)
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{dateType}-{date}-{future}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    /// <inheritdoc/>
    public async Task<string> GetTrendsEtagAsync(Region region, bool nhse)
    {
        var lastModified = await GetLastModifiedAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        var startDate = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{region}-{nhse}-{startDate}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }
}
