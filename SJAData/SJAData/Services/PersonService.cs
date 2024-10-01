// <copyright file="PersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SJAData.Client.Services.Interfaces;
using SJAData.Data;

namespace SJAData.Services;

/// <summary>
/// A service to manage people.
/// </summary>
public class PersonService(ApplicationDbContext context) : IPersonService
{
    private readonly ApplicationDbContext context = context;

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        var hoursLastModified = await context.Hours.Select(s => s.UpdatedAt).DefaultIfEmpty(DateTimeOffset.MinValue).MaxAsync();
        var peopleLastModified = await context.People.Select(s => s.UpdatedAt).DefaultIfEmpty(DateTimeOffset.MinValue).MaxAsync();

        return DateTimeOffset.Compare(hoursLastModified, peopleLastModified) > 0 ? hoursLastModified : peopleLastModified;
    }

    /// <inheritdoc/>
    //public Task<PeopleReportsResponse> GetPeopleReportsAsync(DateOnly date, Grpc.Region region)
    //{
    //    var people = context.People
    //        .AsNoTracking()
    //        .Where(p => p.Region == (Data.Region)region && p.DeletedAt == null)
    //        .Include(p => p.Hours)
    //        .Select(p => new
    //        {
    //            Name = $"{p.FirstName} {p.LastName}",
    //            Hours = p.Hours.Where(h => h.DeletedAt == null && h.Date <= date && Math.Abs(EF.Functions.DateDiffMonth(date, h.Date)) < 13).ToList(),
    //        });

    //    var result = new PeopleReportsResponse();

    //    foreach (var p in people)
    //    {
    //        var report = new PersonReport
    //        {
    //            Name = p.Name,
    //            HoursThisYear = (uint)Math.Round(p.Hours.Where(p => p.Date.Year == DateTime.Now.Year).Select(h => h.Hours).Sum()),
    //            MonthsSinceLastActive = (uint)Math.Round((DateTime.Today.Date - p.Hours.Select(h => h.Date).DefaultIfEmpty(DateOnly.MinValue).Max(h => h).ToDateTime(new TimeOnly(0, 0, 0))).TotalDays / 28),
    //        };
    //        report.Hours.AddRange(GetOverTime(p.Hours));
    //    }

    //    return Task.FromResult(result);
    //}

    /// <inheritdoc/>
    //public async Task<string> GetPeopleReportsEtagAsync(DateOnly date, Grpc.Region region)
    //{
    //    var lastModified = await GetLastModifiedAsync();

    //    var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{date}-{region}-{lastModified}"));

    //    return $"\"{Convert.ToBase64String(hash)}\"";
    //}

    //private static uint[] GetOverTime(IEnumerable<HoursEntry> hours)
    //{
    //    var startDate = DateOnly.FromDateTime(DateTime.Now);
    //    var endDate = startDate.AddMonths(-12);

    //    // Create an array to hold 12 months of data
    //    uint[] monthlyHours = new uint[12];

    //    // Fetch the relevant data, grouped by year and month
    //    var details = hours
    //        .Where(h => h.Date <= startDate && h.Date >= endDate)
    //        .GroupBy(h => new { h.Date.Year, h.Date.Month })
    //        .Select(g => new { g.Key.Year, g.Key.Month, HoursSum = g.Sum(h => h.Hours) });

    //    // Iterate over the details and map them to the correct months
    //    foreach (var detail in details)
    //    {
    //        var index = 11 - (((startDate.Year - detail.Year) * 12) + startDate.Month - detail.Month);
    //        if (index >= 0 && index < 12)
    //        {
    //            monthlyHours[index] = (uint)Math.Round(detail.HoursSum);
    //        }
    //    }

    //    return monthlyHours;
    //}
}
