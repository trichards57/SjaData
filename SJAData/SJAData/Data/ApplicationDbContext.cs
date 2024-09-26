// <copyright file="ApplicationDbContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SJAData.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary>
    /// Gets or sets the hours entries.
    /// </summary>
    public DbSet<HoursEntry> Hours { get; set; }

    /// <summary>
    /// Gets or sets the people.
    /// </summary>
    public DbSet<Person> People { get; set; }
}
