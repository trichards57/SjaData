// <copyright file="IHoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Primitives;
using SjaInNumbers2.Client.Model;
using SjaInNumbers2.Client.Model.Hours;
using SjaInNumbers2.Client.Model.Trends;
using SjaInNumbers2.Model.Hours;

namespace SjaInNumbers2.Client.Services.Interfaces;

/// <summary>
/// Represents a service for managing hours entries.
/// </summary>
public interface IHoursService
{
    Task<int> AddHours(IAsyncEnumerable<HoursFileLine> asyncEnumerable, string userId);

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
    Task<StringSegment> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future);
    Task<DateTimeOffset?> GetLastModifiedAsync();

    /// <summary>
    /// Gets the current NHSE target.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the target.
    /// </returns>
    Task<int> GetNhseTargetAsync();
    Task<StringSegment> GetNhseTargetEtagAsync();
    Task<DateTimeOffset?> GetNhseTargetLastModifiedAsync();

    /// <summary>
    /// Gets the activity trends for a region.
    /// </summary>
    /// <param name="region">The region to query for.</param>
    /// <param name="nhse">Indicates that only NSHE data should be returned.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the trends report.
    /// </returns>
    Task<Trends> GetTrendsAsync(Region region, bool nhse);
}
