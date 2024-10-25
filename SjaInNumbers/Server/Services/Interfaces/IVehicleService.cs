// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;

namespace SjaInNumbers.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing vehicles.
/// </summary>
public interface IVehicleService
{
    /// <summary>
    /// Adds the provided VOR incidents to the database.
    /// </summary>
    /// <param name="vorIncidents">The incidents to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddEntriesAsync(IEnumerable<VorIncident> vorIncidents);
    IAsyncEnumerable<VehicleTypeStatus> GetNationalVorStatusesAsync();

    /// <summary>
    /// Gets the settings for all vehicles at the provided place.
    /// </summary>
    /// <param name="place">The place to filter by.</param>
    /// <returns>The list of vehicles.</returns>
    IAsyncEnumerable<VehicleSettings> GetSettingsAsync(Place place);

    /// <summary>
    /// Gets the settings for the vehicle with the provided ID.
    /// </summary>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves
    /// to the requested settings, or <see langword="null" /> if not found.
    /// </returns>
    Task<VehicleSettings?> GetSettingsAsync(int id);

    /// <summary>
    /// Gets the VOR statistics for the provided place.
    /// </summary>
    /// <param name="place">The place to search for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves
    /// to the requested statistics, or <see langword="null" /> if the place was not found.
    /// </returns>
    Task<VorStatistics?> GetVorStatisticsAsync(Place place);

    /// <summary>
    /// Gets the VOR statuses for the provided place.
    /// </summary>
    /// <param name="place">The place to search for.</param>
    /// <returns>The list of statuses.</returns>
    IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Place place);

    /// <summary>
    /// Updates or creates the settings for the provided vehicle.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PutSettingsAsync(UpdateVehicleSettings settings);
}
