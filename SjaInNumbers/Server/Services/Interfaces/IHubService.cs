// <copyright file="IHubService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing hubs.
/// </summary>
public interface IHubService
{
    /// <summary>
    /// Adds a hub to the system.
    /// </summary>
    /// <param name="newHub">The details of the new hub.</param>
    /// <returns>A summary of the new hub.</returns>
    Task<HubSummary> AddHubAsync(NewHub newHub);

    /// <summary>
    /// Removes a hub from the system.
    /// </summary>
    /// <param name="id">The ID of the hub.</param>
    /// <returns>
    /// <see langword="true" /> if the hub was removed; otherwise, <see langword="false" />.
    /// </returns>
    Task<bool> DeleteHubAsync(int id);

    /// <summary>
    /// Gets all of the registered hubs.
    /// </summary>
    /// <returns>A list of all hubs.</returns>
    IAsyncEnumerable<HubSummary> GetAllAsync();

    /// <summary>
    /// Gets the ETAG for the list of all hubs.
    /// </summary>
    /// <returns>An ETag representing the current data.</returns>
    Task<string> GetAllEtagAsync();

    /// <summary>
    /// Gets the last modified date for the hub data.
    /// </summary>
    /// <returns>The date the hub data was last updated.</returns>
    Task<DateTimeOffset> GetLastModifiedAsync();

    /// <summary>
    /// Gets the name of the given hub.
    /// </summary>
    /// <param name="id">The ID of the hub.</param>
    /// <returns>The hub's name.</returns>
    Task<string?> GetNameAsync(int id);

    /// <summary>
    /// Sets the name of the given hub.
    /// </summary>
    /// <param name="id">The ID of the hub.</param>
    /// <param name="name">The new name.</param>
    /// <returns>
    /// <see langword="true"/> if the name was set; otherwise, <see langword="false"/>.
    /// </returns>
    Task<bool> SetNameAsync(int id, string name);
}
