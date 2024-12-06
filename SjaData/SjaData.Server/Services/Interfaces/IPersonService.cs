// <copyright file="IPersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model;
using SjaData.Server.Model.People;

namespace SjaData.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing people.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Gets the people activity reports for a specific end-date and region.
    /// </summary>
    /// <param name="date">The date of the last activity.</param>
    /// <param name="region">The region to report on.</param>
    /// <returns>
    /// The people activity reports.
    /// </returns>
    IAsyncEnumerable<PersonReport> GetPeopleReportsAsync(DateOnly date, Region region);

    /// <summary>
    /// Adds a collection of people to the database.
    /// </summary>
    /// <param name="people">The people to add.</param>
    /// <param name="userId">The ID of the user making the change.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the
    /// number of people added.
    /// </returns>
    Task<int> AddPeopleAsync(IAsyncEnumerable<PersonFileLine> people, string userId);

    /// <summary>
    /// Gets the last modified date for the people data.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the
    /// last modified date.
    /// </returns>
    Task<DateTimeOffset> GetLastModifiedAsync();

    /// <summary>
    /// Gets the current Etag for the people data.
    /// </summary>
    /// <param name="date">The report date for the data.</param>
    /// <param name="region">The region to report on.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the
    /// Etag.
    /// </returns>
    Task<string> GetPeopleReportsEtagAsync(DateOnly date, Region region);
}
