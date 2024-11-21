// <copyright file="IDistrictsService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Client.Services.Interfaces;

/// <summary>
/// Represents a service for interacting with districts.
/// </summary>
public interface IDistrictsService
{
    /// <summary>
    /// Gets a district by its ID.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The task result contains the district.
    /// </returns>
    Task<DistrictSummary> GetDistrictAsync(int id);

    /// <summary>
    /// Gets the summaries of all districts.
    /// </summary>
    /// <returns>
    /// The summaries of all districts.
    /// </returns>
    IAsyncEnumerable<DistrictSummary> GetDistrictSummariesAsync();

    /// <summary>
    /// Updates a district's short-code.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <param name="code">The new code.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task PostDistrictCode(int id, string code);

    /// <summary>
    /// Merges two districts.
    /// </summary>
    /// <param name="sourceId">The ID of the source district.</param>
    /// <param name="destinationId">The ID of the destination district.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task PostDistrictMerge(int sourceId, int destinationId);

    /// <summary>
    /// Updates a district's name.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <param name="name">The new name.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task PostDistrictName(int id, string name);
}
