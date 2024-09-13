// <copyright file="Person.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model;
using System.ComponentModel.DataAnnotations;

namespace SjaData.Server.Data;

/// <summary>
/// Represents a person who does hours.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets or sets the unique identifier for the person.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the clinical role for the person.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region for the person.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the district for the person.
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hours the person has done.
    /// </summary>
    public ICollection<HoursEntry> Hours { get; set; } = [];

    /// <summary>
    /// Gets or sets the date the person was created.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date the person was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }
}
