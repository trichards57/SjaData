// <copyright file="IHoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Server.Model.Hours;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Hours;
using SjaInNumbers.Shared.Model.Trends;

namespace SjaInNumbers.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing hours entries.
/// </summary>
public interface IHoursService
{
    /// <summary>
    /// Counts the hours that match the given query.
    /// </summary>
    /// <param name="date">The date to filter by.</param>
    /// <param name="dateType">The level the date should be filtered with.</param>
    /// <param name="future">If <see langword="true"/> with only return future records, otherwise only returns present and past.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the count.
    /// </returns>
    Task<HoursCount> CountAsync(DateOnly? date, DateType? dateType = DateType.Month, bool future = false);

    /// <summary>
    /// Gets the current NHSE target.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the target.
    /// </returns>
    Task<int> GetNhseTargetAsync();

    /// <summary>
    /// Gets the activity trends for a region.
    /// </summary>
    /// <param name="region">The region to query for.</param>
    /// <param name="nhse">Indicates that only NSHE data should be returned.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the trends report.
    /// </returns>
    Task<Trends> GetTrendsAsync(Region region, bool nhse);

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

    /// <summary>
    /// Gets the current Etag for the NHSE data.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the Etag.
    /// </returns>
    Task<string> GetNhseTargetEtagAsync();

    /// <summary>
    /// Gets the last modified date for the NHSE data.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the date.
    /// </returns>
    Task<DateTimeOffset> GetNhseTargetLastModifiedAsync();

    /// <summary>
    /// Adds hours data to the database.
    /// </summary>
    /// <param name="hours">The hours data to add.</param>
    /// <param name="userId">The ID of the user adding the data.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the
    /// number of entries added.
    /// </returns>
    Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours, string userId);

    /// <summary>
    /// Calculates the ETag associated with an activity report.
    /// </summary>
    /// <param name="region">The region to query for.</param>
    /// <param name="nhse">Indicates that only NSHE data should be returned.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the ETag.
    /// </returns>
    Task<string> GetTrendsEtagAsync(Region region, bool nhse);
}
