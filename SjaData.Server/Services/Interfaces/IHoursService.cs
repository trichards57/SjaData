﻿// <copyright file="IHoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model;
using SjaData.Server.Model.Hours;

namespace SjaData.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing hours entries.
/// </summary>
public interface IHoursService
{
    /// <summary>
    /// Adds or updates a list of hours entries.
    /// </summary>
    /// <param name="hours">The new hours data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the number of changed items.</returns>
    Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours);

    /// <summary>
    /// Counts the hours that match the given query.
    /// </summary>
    /// <param name="date">The date to filter by.</param>
    /// <param name="dateType">The level the date should be filtered with.</param>
    /// <param name="future">If <see langword="true"/> with only return future records, otherwise only returns present and past.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the count.
    /// </returns>
    Task<HoursCount> CountAsync(DateOnly? date, DateType? dateType, bool future);

    /// <summary>
    /// Deletes the given hours entry.
    /// </summary>
    /// <param name="id">The ID of the entry to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteAsync(int id);

    /// <summary>
    /// Calculates the ETag associated with an hours count.
    /// </summary>
    /// <param name="date">The date of the report.</param>
    /// <param name="dateType">The date type for the report.</param>
    /// <param name="future">Indicates if only future information should be included.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the ETag.
    /// </returns>
    /// <remarks>
    /// This is a weak ETag and should be marked accordingly.
    /// </remarks>
    Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future);

    /// <summary>
    /// Gets the date the hours data was last modified.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the date.
    /// </returns>
    Task<DateTimeOffset> GetLastModifiedAsync();

    Task<Trends> GetTrendsAsync(Region region, bool nhse);

    Task<string> GetTrendsEtagAsync(Region region, bool nhse);

}
