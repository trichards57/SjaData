// <copyright file="ApplicationDbContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SjaInNumbers.Server.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
{
    /// <summary>
    /// Gets or sets the hours entries.
    /// </summary>
    public DbSet<HoursEntry> Hours { get; set; }

    /// <summary>
    /// Gets or sets the people.
    /// </summary>
    public DbSet<Person> People { get; set; }

    /// <summary>
    /// Gets or sets the vehicles.
    /// </summary>
    public DbSet<Vehicle> Vehicles { get; set; }

    /// <summary>
    /// Gets or sets the incidents associated with vehicles.
    /// </summary>
    public DbSet<VehicleIncident> VehicleIncidents { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = "1",
                Name = "Admin",
                NormalizedName = "ADMIN",
            },
            new IdentityRole
            {
                Id = "2",
                Name = "Lead",
                NormalizedName = "LEAD",
            });
    }
}
