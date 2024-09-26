// <copyright file="IPersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SJAData.Grpc;

namespace SJAData.Client.Services.Interfaces;

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
    Task<PeopleReportsResponse> GetPeopleReportsAsync(DateOnly date, Region region);

    /// <summary>
    /// Gets the current ETag for the people activity reports for a specific end-date and region.
    /// </summary>
    /// <param name="date">The date of the last activity.</param>
    /// <param name="region">The region to report on.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the ETag.
    /// </returns>
    Task<string> GetPeopleReportsEtagAsync(DateOnly date, Region region);

    /// <summary>
    /// Gets the date the source data was last modified.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the date.
    /// </returns>
    Task<DateTimeOffset> GetLastModifiedAsync();
}
