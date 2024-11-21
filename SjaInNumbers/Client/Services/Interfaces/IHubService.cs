// <copyright file="IHubService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Client.Services.Interfaces;

/// <summary>
/// Represents a service for interacting with hubs.
/// </summary>
public interface IHubService
{
    /// <summary>
    /// Gets the name of the given hub.
    /// </summary>
    /// <param name="id">The ID of the hub.</param>
    /// <returns>The hub's name.</returns>
    Task<string> GetHubNameAsync(int id);

    /// <summary>
    /// Updates the name of the given hub.
    /// </summary>
    /// <param name="id">The ID of the hub.</param>
    /// <param name="name">The new name.</param>
    /// <returns>A task representing the asynchronous activity.</returns>
    Task PostHubNameAsync(int id, string name);

    /// <summary>
    /// Gets a list of all hubs.
    /// </summary>
    /// <returns>The list of hubs.</returns>
    IAsyncEnumerable<HubSummary> GetHubSummariesAsync();

    /// <summary>
    /// Posts a new hub to the server.
    /// </summary>
    /// <param name="districtId">The ID of the district for the new hub.</param>
    /// <param name="name">The name of the new hub.</param>
    /// <returns>A task representing the asynchronous activity.</returns>
    Task PostHubAsync(int districtId, string name);
}
