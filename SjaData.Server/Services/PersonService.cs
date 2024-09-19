﻿// <copyright file="PersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Model;
using SjaData.Server.Model.People;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

/// <summary>
/// A service to manage people.
/// </summary>
public class PersonService(DataContext context) : IPersonService
{
    private readonly DataContext context = context;

    public async Task<IEnumerable<PersonReport>> GetPeopleReports(Region region)
    {
        var people = await context.People
            .AsNoTracking()
            .Where(p => p.Region == region && p.DeletedAt == null)
            .Include(p => p.Hours)
            .Select(p => new
            {
                Name = $"{p.FirstName} {p.LastName}",
                Hours = p.Hours.Where(h => h.DeletedAt == null && h.PersonId == p.Id && Math.Abs(EF.Functions.DateDiffMonth(DateOnly.FromDateTime(DateTime.Today), h.Date)) < 13).ToList(),
            }).ToListAsync();

        return people.Select(p => new PersonReport
        {
            Name = p.Name,
            Hours = GetOverTime(p.Hours),
            HoursThisYear = p.Hours.Where(p => p.Date.Year == DateTime.Now.Year).Select(h => h.Hours).Sum(),
            MonthsSinceLastActive = (int)Math.Round((DateTime.Today.Date - p.Hours.Select(h => h.Date).DefaultIfEmpty(DateOnly.MinValue).Max(h => h).ToDateTime(new TimeOnly(0, 0, 0))).TotalDays / 28),
        });
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
            var index = 11 - (((startDate.Year - detail.Year) * 12) + startDate.Month - detail.Month);
            if (index >= 0 && index < 12)
            {
                monthlyHours[index] = detail.HoursSum;
            }
        }

        return monthlyHours;
    }

    /// <inheritdoc/>
    public async Task<int> AddPeople(IAsyncEnumerable<PersonFileLine> people)
    {
        var peopleList = await people.Where(p => p.JobRoleTitle.Equals("emergency ambulance crew", StringComparison.InvariantCultureIgnoreCase)).Select(p =>
        {
            var name = p.Name.Split(',');
            return new Person
            {
                Id = p.MyDataNumber,
                FirstName = name[1].Trim(),
                LastName = name[0].Trim(),
                District = (p.DistrictStation.StartsWith("District: ") ? p.DistrictStation.Substring(10) : p.DistrictStation).Trim(),
                Role = p.JobRoleTitle,
                Region = CalculateRegion(p),
                IsVolunteer = p.IsVolunteer,
            };
        }).ToListAsync();

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

                if (existingPerson.District != p.District)
                {
                    existingPerson.District = p.District;
                }

                if (existingPerson.Role != p.Role)
                {
                    existingPerson.Role = p.Role;
                }

                if (existingPerson.Region != p.Region)
                {
                    existingPerson.Region = p.Region;
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
                }
            }
            else
            {
                p.UpdatedAt = DateTimeOffset.UtcNow;
                context.People.Add(p);
            }
        }

        foreach (var p in existingPeople)
        {
            if (!peopleList.Exists(q => q.Id == p.Key))
            {
                p.Value.UpdatedAt = DateTimeOffset.UtcNow;
                p.Value.DeletedAt = DateTimeOffset.UtcNow;
            }
        }

        return await context.SaveChangesAsync();
    }

    private static Region CalculateRegion(Model.People.PersonFileLine person)
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
}