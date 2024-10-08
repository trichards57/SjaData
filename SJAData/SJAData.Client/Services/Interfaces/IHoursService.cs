﻿// <copyright file="IHoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SJAData.Client.Data;
using SJAData.Client.Model;
using SJAData.Client.Model.Hours;
using SJAData.Client.Model.Trends;

namespace SJAData.Client.Services.Interfaces;

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
    /// Deletes the given hours entry.
    /// </summary>
    /// <param name="id">The ID of the entry to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    //Task DeleteAsync(int id);

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

 
}
