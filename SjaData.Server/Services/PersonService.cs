// <copyright file="PersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Model;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

/// <summary>
/// A service to manage people.
/// </summary>
public class PersonService(DataContext context) : IPersonService
{
    private readonly DataContext context = context;

    /// <inheritdoc/>
    public async Task<int> AddPeople(IAsyncEnumerable<Model.People.PersonFileLine> people)
    {
        var peopleList = await people.Where(p => p.JobRoleTitle.Equals("emergency ambulance crew", StringComparison.InvariantCultureIgnoreCase)).Select(p =>
        {
            var name = p.Name.Split(',');
            return new Data.Person
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
