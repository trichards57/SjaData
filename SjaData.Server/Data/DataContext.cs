// <copyright file="DataContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;

namespace SjaData.Server.Data;

/// <summary>
/// Main data context for the application.
/// </summary>
/// <param name="options">Options to configure the data context.</param>
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the user entries.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the hours entries.
    /// </summary>
    public DbSet<HoursEntry> Hours { get; set; }

    /// <summary>
    /// Gets or sets the patients.
    /// </summary>
    public DbSet<Patient> Patients { get; set; }

    /// <summary>
    /// Gets or sets the people.
    /// </summary>
    public DbSet<Person> People { get; set; }
}
