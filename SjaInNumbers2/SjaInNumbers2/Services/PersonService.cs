// <copyright file="PersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Model.People;
using SjaInNumbers2.Client.Model;
using SjaInNumbers2.Client.Model.People;
using SjaInNumbers2.Client.Services.Interfaces;
using SjaInNumbers2.Data;
using System.Security.Cryptography;
using System.Text;

namespace SjaInNumbers2.Services;

/// <summary>
/// A service to manage people.
/// </summary>
public class PersonService(IDistrictsService districtService, IDbContextFactory<ApplicationDbContext> contextFactory) : IPersonService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextBuilder = contextFactory;
    private readonly IDistrictsService districtService = districtService;

    /// <inheritdoc/>
    public async Task<int> AddPeopleAsync(IAsyncEnumerable<PersonFileLine> people, string userId)
    {
        using var context = await contextBuilder.CreateDbContextAsync();
        var peopleList = new List<Person>();

        await foreach (var p in people)
        {
            var district = (p.DistrictStation.StartsWith("District: ") ? p.DistrictStation[10..] : p.DistrictStation).Trim();
            var region = CalculateRegion(p);

            var districtId = await districtService.GetIdByNameAsync(district, region);

            var place = districtId.HasValue ? await context.Hubs.FirstOrDefaultAsync(h => h.DistrictId == districtId) : null;

            if (place == null)
            {
                var newDistrict = new District { Name = district, Region = region };
                place = new Hub { Name = $"AutoHubFor_{district}", District = newDistrict };

                context.Districts.Add(newDistrict);
                context.Hubs.Add(place);
                await context.SaveChangesAsync();
            }

            var name = p.Name.Split(',');

            peopleList.Add(new Person
            {
                Id = p.MyDataNumber,
                FirstName = name[1].Trim(),
                LastName = name[0].Trim(),
                HubId = place.Id,
                Role = p.JobRoleTitle,
                IsVolunteer = p.IsVolunteer,
            });
        }

        var existingPeople = await context.People.ToDictionaryAsync(p => p.Id);

        foreach (var p in peopleList)
        {
            if (existingPeople.TryGetValue(p.Id, out var existingPerson))
            {
                if (existingPerson.FirstName != p.FirstName)
                {
                    existingPerson.FirstName = p.FirstName;
                }

                if (existingPerson.LastName != p.LastName)
                {
                    existingPerson.LastName = p.LastName;
                }

                if (existingPerson.HubId != p.HubId)
                {
                    existingPerson.Hub = p.Hub;
                    existingPerson.HubId = p.HubId;
                }

                if (existingPerson.Role != p.Role)
                {
                    existingPerson.Role = p.Role;
                }

                if (existingPerson.IsVolunteer != p.IsVolunteer)
                {
                    existingPerson.IsVolunteer = p.IsVolunteer;
                }

                if (existingPerson.DeletedAt != null)
                {
                    existingPerson.DeletedAt = null;
                }

                if (context.Entry(existingPerson).State == EntityState.Modified)
                {
                    existingPerson.UpdatedAt = DateTimeOffset.UtcNow;
                    existingPerson.UpdatedById = userId;
                }
            }
            else
            {
                p.UpdatedAt = DateTimeOffset.UtcNow;
                p.UpdatedById = userId;
                context.People.Add(p);
                existingPeople.Add(p.Id, p);
            }
        }

        foreach (var p in existingPeople)
        {
            if (!peopleList.Exists(q => q.Id == p.Key))
            {
                p.Value.UpdatedAt = DateTimeOffset.UtcNow;
                p.Value.DeletedAt = p.Value.UpdatedAt;
                p.Value.UpdatedById = userId;
            }
        }

        return await context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        using var context = await contextBuilder.CreateDbContextAsync();
        var hoursLastModified = await context.Hours.AnyAsync() ? await context.Hours.Select(s => s.UpdatedAt).MaxAsync() : DateTimeOffset.MinValue;
        var peopleLastModified = await context.People.AnyAsync() ? await context.People.Select(s => s.UpdatedAt).MaxAsync() : DateTimeOffset.MinValue;
        var hubsLastModified = await context.Hubs.AnyAsync() ? await context.Hubs.Select(s => s.UpdatedAt).MaxAsync() : DateTimeOffset.MinValue;

        return new[] { hoursLastModified, peopleLastModified, hubsLastModified }.Max();
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<PersonReport> GetPeopleReportsAsync(DateOnly date, Region region)
    {
        using var context = await contextBuilder.CreateDbContextAsync();
        var people = context.People
            .AsNoTracking()
            .Where(p => p.Hub != null && p.Hub.District.Region == region && p.DeletedAt == null)
            .Include(p => p.Hours)
            .Include(p => p.Hub)
            .Select(p => new
            {
                Name = $"{p.FirstName} {p.LastName}",
                Hours = p.Hours.Where(h => h.DeletedAt == null && h.Date <= date && Math.Abs(EF.Functions.DateDiffMonth(date, h.Date)) < 13).ToList(),
                District = p.Hub != null ? p.Hub.District.Name : string.Empty,
                Hub = p.Hub != null ? p.Hub.Name : string.Empty,
            }).AsAsyncEnumerable();

        await foreach (var p in people)
        {
            var report = new PersonReport
            {
                Name = p.Name,
                HoursThisYear = (uint)Math.Round(p.Hours.Where(p => p.Date.Year == DateTime.Now.Year).Select(h => h.Hours).Sum()),
                MonthsSinceLastActive = (int)Math.Round((DateTime.Today.Date - p.Hours.Select(h => h.Date).DefaultIfEmpty(DateOnly.MinValue).Max(h => h).ToDateTime(new TimeOnly(0, 0, 0))).TotalDays / 28),
                Hours = GetOverTime(p.Hours),
                District = p.District,
                Hub = p.Hub,
            };

            yield return report;
        }
    }

    /// <inheritdoc/>
    public async Task<string> GetPeopleReportsEtagAsync(DateOnly date, Region region)
    {
        var lastModified = await GetLastModifiedAsync();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{date}-{region}-{lastModified}"));

        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    private static Region CalculateRegion(PersonFileLine person)
    {
        return person.DepartmentRegion.ToLowerInvariant() switch
        {
            "london region" => Region.London,
            "events: london" => Region.London,
            "east of england region" => Region.EastOfEngland,
            "north east region" => Region.NorthEast,
            "south east region" => Region.SouthEast,
            "west midlands region" => Region.WestMidlands,
            "east midlands region" => Region.EastMidlands,
            "south west region" => Region.SouthWest,
            "north west region" => Region.NorthWest,
            _ => Region.Undefined,
        };
    }

    private static double[] GetOverTime(IEnumerable<HoursEntry> hours)
    {
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = startDate.AddMonths(-12);

        // Create an array to hold 12 months of data
        double[] monthlyHours = new double[12];

        // Fetch the relevant data, grouped by year and month
        var details = hours
            .Where(h => h.Date <= startDate && h.Date >= endDate)
            .GroupBy(h => new { h.Date.Year, h.Date.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, HoursSum = g.Sum(h => h.Hours) });

        // Iterate over the details and map them to the correct months
        foreach (var detail in details)
        {
            var index = 11 - ((startDate.Year - detail.Year) * 12 + startDate.Month - detail.Month);
            if (index >= 0 && index < 12)
            {
                monthlyHours[index] = detail.HoursSum;
            }
        }

        return monthlyHours;
    }
}
