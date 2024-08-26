// <copyright file="IHoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model.Hours;
using SjaData.Server.Api.Model;

namespace SjaData.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing hours entries.
/// </summary>
public interface IHoursService
{
    /// <summary>
    /// Adds or updates a new hours entry.
    /// </summary>
    /// <param name="hours">The new hours data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddAsync(NewHoursEntry hours);

    /// <summary>
    /// Counts the hours that match the given query.
    /// </summary>
    /// <param name="query">The query to search for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the count.
    /// </returns>
    Task<HoursCount> CountAsync(HoursQuery query);

    /// <summary>
    /// Deletes the given hours entry.
    /// </summary>
    /// <param name="id">The ID of the entry to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteAsync(int id);

    /// <summary>
    /// Gets the date the hours data was last modified.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the date.
    /// </returns>
    Task<DateTimeOffset> GetLastModifiedAsync();
}
