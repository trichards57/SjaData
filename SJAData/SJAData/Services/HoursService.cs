// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SJAData.Client.Services.Interfaces;
using SJAData.Data;


namespace SJAData.Services;

/// <summary>
/// Service for managing hours entries.
/// </summary>
/// <param name="dataContext">The data context containing the hours data.</param>
/// <param name="logger">The logger to write to.</param>
public partial class HoursService(TimeProvider timeProvider, ApplicationDbContext dataContext, ILogger<HoursService> logger) : IHoursService
{
    private readonly ApplicationDbContext dataContext = dataContext;
    private readonly ILogger<HoursService> logger = logger;
    private readonly TimeProvider timeProvider = timeProvider;

    /// <inheritdoc/>
    //public async Task<HoursCountResponse> CountAsync(DateOnly? date, DateType? dateType, bool future)
    //{
    //    var items = dataContext.Hours.AsNoTracking()
    //        .Where(i => i.DeletedAt == null && i.Person.IsVolunteer && (i.Region != Data.Region.Undefined || i.Trust != Trust.Undefined));

    //    if (!date.HasValue)
    //    {
    //        date = DateOnly.FromDateTime(DateTime.Today);
    //        dateType = DateType.Year;
    //    }

    //    items = dateType switch
    //    {
    //        DateType.Day => items.Where(h => h.Date == date.Value),
    //        DateType.Month => items.Where(h => h.Date.Month == date.Value.Month && h.Date.Year == date.Value.Year),
    //        _ => items.Where(h => h.Date.Year == date.Value.Year),
    //    };

    //    if (future)
    //    {
    //        items = items.Where(d => d.Date > date);
    //    }
    //    else
    //    {
    //        items = items.Where(d => d.Date <= date);
    //    }

    //    var hoursCount = (await items.Select(h => new
    //    {
    //        h.Region,
    //        h.Trust,
    //        h.Hours,
    //    }).ToListAsync()).Select(h => new
    //    {
    //        Label = h.Trust == Trust.Undefined ? RegionConverter.ToString(h.Region) : TrustConverter.ToString(h.Trust),
    //        h.Hours,
    //    })
    //    .Where(c => !string.IsNullOrEmpty(c.Label))
    //    .GroupBy(h => h.Label)
    //    .ToDictionary(h => h.Key, h => TimeSpan.FromHours(h.Sum(i => i.Hours)));
    //    var lastUpdate = await GetLastModifiedAsync();

    //    var result = new HoursCountResponse
    //    {
    //        LastUpdate = Timestamp.FromDateTimeOffset(lastUpdate),
    //    };

    //    foreach (var kvp in hoursCount)
    //    {
    //        result.Counts.Add(kvp.Key, (uint)kvp.Value.TotalHours);
    //    }

    //    return result;
    //}

    ///// <inheritdoc/>
    //public async Task DeleteAsync(int id)
    //{
    //    var existingItem = await dataContext.Hours.FirstOrDefaultAsync(h => h.Id == id && !h.DeletedAt.HasValue);

    //    if (existingItem is not null)
    //    {
    //        existingItem.UpdatedAt = DateTimeOffset.UtcNow;
    //        existingItem.DeletedAt = existingItem.UpdatedAt;
    //        await dataContext.SaveChangesAsync();
    //    }
    //}

    ///// <inheritdoc/>
    //public async Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future)
    //{
    //    var lastModified = await GetLastModifiedAsync();

    //    var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{dateType}-{date}-{future}-{lastModified}"));

    //    return $"\"{Convert.ToBase64String(hash)}\"";
    //}

    ///// <inheritdoc/>
    //public async Task<DateTimeOffset> GetLastModifiedAsync()
    //{
    //    if (await dataContext.Hours.AnyAsync())
    //    {
    //        return await dataContext.Hours.MaxAsync(p => p.DeletedAt ?? p.UpdatedAt);
    //    }

    //    return DateTimeOffset.MinValue;
    //}

    ///// <inheritdoc/>
    //public async Task<HoursTrendsResponse> GetTrendsAsync(Grpc.Region region, bool nhse)
    //{
    //    var districts = await dataContext.People.Where(p => p.Region == (Data.Region)region).Select(p => p.District).Distinct().ToListAsync();

    //    var today = DateOnly.FromDateTime(DateTime.Today);
    //    var reportDate = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

    //    IQueryable<HoursEntry> hours = dataContext.Hours.AsNoTracking();

    //    if (nhse)
    //    {
    //        hours = hours.Where(h => h.Trust != Trust.Undefined);
    //    }
    //    else
    //    {
    //        hours = hours.Where(h => h.Region != Data.Region.Undefined || h.Trust != Trust.Undefined);
    //    }

    //    var nationalAverages = await GetAverages(hours, reportDate);
    //    var nationalAveragesMinusOne = await GetAverages(hours, reportDate.AddMonths(-1));
    //    var regionAverages = await GetAverages(hours.Where(h => h.Person.Region == (Data.Region)region), reportDate);
    //    var regionAveragesMinusOne = await GetAverages(hours.Where(h => h.Person.Region == (Data.Region)region), reportDate.AddMonths(-1));

    //    var trends = new HoursTrendsResponse()
    //    {
    //        ReportDate = Timestamp.FromDateTimeOffset(reportDate.ToDateTime(TimeOnly.MinValue)),
    //    };
    //    trends.TwelveMonthAverage["national"] = (uint)nationalAverages.TwelveMonthAverage;
    //    trends.TwelveMonthAverage["region"] = (uint)regionAverages.TwelveMonthAverage;
    //    trends.TwelveMonthMinusOneAverage["national"] = (uint)nationalAveragesMinusOne.TwelveMonthAverage;
    //    trends.TwelveMonthMinusOneAverage["region"] = (uint)regionAveragesMinusOne.TwelveMonthAverage;
    //    trends.SixMonthAverage["national"] = (uint)nationalAverages.SixMonthAverage;
    //    trends.SixMonthAverage["region"] = (uint)regionAverages.SixMonthAverage;
    //    trends.SixMonthMinusOneAverage["national"] = (uint)nationalAveragesMinusOne.SixMonthAverage;
    //    trends.SixMonthMinusOneAverage["region"] = (uint)regionAveragesMinusOne.SixMonthAverage;
    //    trends.ThreeMonthAverage["national"] = (uint)nationalAverages.ThreeMonthAverage;
    //    trends.ThreeMonthAverage["region"] = (uint)regionAverages.ThreeMonthAverage;
    //    trends.ThreeMonthMinusOneAverage["national"] = (uint)nationalAveragesMinusOne.ThreeMonthAverage;
    //    trends.ThreeMonthMinusOneAverage["region"] = (uint)regionAveragesMinusOne.ThreeMonthAverage;
    //    trends.Hours["national"] = await GetOverTime(hours, reportDate);
    //    trends.Hours["region"] = await GetOverTime(hours.Where(h => h.Person.Region == (Data.Region)region), reportDate);

    //    foreach (var d in districts)
    //    {
    //        try
    //        {
    //            var districtAverages = await GetAverages(hours.Where(h => h.Person.District == d), reportDate);
    //            var districtAverageMinusOne = await GetAverages(hours.Where(h => h.Person.District == d), reportDate.AddMonths(-1));

    //            trends.TwelveMonthAverage[d] = (uint)districtAverages.TwelveMonthAverage;
    //            trends.TwelveMonthMinusOneAverage[d] = (uint)districtAverageMinusOne.TwelveMonthAverage;
    //            trends.SixMonthAverage[d] = (uint)districtAverages.SixMonthAverage;
    //            trends.SixMonthMinusOneAverage[d] = (uint)districtAverageMinusOne.SixMonthAverage;
    //            trends.ThreeMonthAverage[d] = (uint)districtAverages.ThreeMonthAverage;
    //            trends.ThreeMonthMinusOneAverage[d] = (uint)districtAverageMinusOne.ThreeMonthAverage;
    //            trends.Hours[d] = await GetOverTime(hours.Where(h => h.Person.District == d), reportDate);
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.LogError(ex, "Error processing district {District}", d);
    //        }
    //    }

    //    return trends;
    //}

    ///// <inheritdoc/>
    //public async Task<string> GetTrendsEtagAsync(Grpc.Region region, bool nhse)
    //{
    //    ExceptionHelpers.ThrowIfUndefined(region);

    //    var lastModified = await GetLastModifiedAsync();

    //    var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().Date);
    //    var startDate = new DateOnly(today.Year, today.Month, 1).AddDays(-1);

    //    var marker = $"{region}-{nhse}-{startDate}-{lastModified}";
    //    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(marker));

    //    return $"\"{Convert.ToBase64String(hash)}\"";
    //}

    public Task<int> GetNhseTargetAsync()
    {
        return Task.FromResult(4000);
    }

    //private static Task<Averages> GetAverages(IQueryable<HoursEntry> hours, DateOnly startDate)
    //{
    //    var endDate = startDate.AddMonths(-12);

    //    return hours
    //        .Where(h => h.DeletedAt == null && h.Person.IsVolunteer)
    //        .Where(h => h.Date <= startDate && h.Date >= endDate)
    //        .GroupBy(h => 1)
    //        .Select(h => new Averages
    //        {
    //            TwelveMonthAverage = h.Select(s => s.Hours).Sum() / 12,
    //            SixMonthAverage = h.Where(i => i.Date >= startDate.AddMonths(-6)).Select(s => s.Hours).Sum() / 6,
    //            ThreeMonthAverage = h.Where(i => i.Date >= startDate.AddMonths(-3)).Select(s => s.Hours).Sum(s => s) / 3,
    //        }).FirstAsync();
    //}

    //private static async Task<HoursList> GetOverTime(IQueryable<HoursEntry> hours, DateOnly startDate)
    //{
    //    var endDate = startDate.AddMonths(-12);

    //    // Create an array to hold 12 months of data
    //    double[] monthlyHours = new double[12];

    //    // Fetch the relevant data, grouped by year and month
    //    var details = await hours
    //        .Where(h => h.DeletedAt == null && h.Person.IsVolunteer)
    //        .Where(h => h.Date <= startDate && h.Date >= endDate)
    //        .GroupBy(h => new { h.Date.Year, h.Date.Month })
    //        .Select(g => new { g.Key.Year, g.Key.Month, HoursSum = g.Sum(h => h.Hours) })
    //        .ToListAsync();

    //    // Iterate over the details and map them to the correct months
    //    foreach (var detail in details)
    //    {
    //        var index = 11 - (((startDate.Year - detail.Year) * 12) + startDate.Month - detail.Month);
    //        if (index >= 0 && index < 12)
    //        {
    //            monthlyHours[index] = detail.HoursSum;
    //        }
    //    }

    //    var result = new HoursList();

    //    for (var i = 0; i < 12; i++)
    //    {
    //        result.Hours.Add((uint)monthlyHours[i]);
    //    }

    //    return result;
    //}

    //private readonly record struct Averages
    //{
    //    public double TwelveMonthAverage { get; init; }

    //    public double SixMonthAverage { get; init; }

    //    public double ThreeMonthAverage { get; init; }
    //}
}
