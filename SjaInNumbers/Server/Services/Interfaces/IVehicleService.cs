// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Primitives;
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

    /// <summary>
    /// Gets the failure reports for a year ending with the provided end date and vehicle type.
    /// </summary>
    /// <param name="endDate">The end date for the failure reports.</param>
    /// <param name="type">The vehicle type.</param>
    /// <returns>The list of failure reports.</returns>
    IAsyncEnumerable<FailureReport> GetFailureReports(DateOnly endDate, VehicleType type);

    /// <summary>
    /// Gets the last modified date for the vehicle data.
    /// </summary>
    /// <returns>The date the vehicle data was last updated.</returns>
    Task<DateTimeOffset> GetLastModifiedAsync();

    /// <summary>
    /// Gets the national VOR statuses.
    /// </summary>
    /// <returns>The list of vehicle statuses.</returns>
    IAsyncEnumerable<VehicleTypeStatus> GetNationalVorStatusesAsync();

    /// <summary>
    /// Gets the ETag for the national VOR statuses.
    /// </summary>
    /// <returns>An ETag representing the current data.</returns>
    Task<StringSegment> GetNationalVorStatusesEtagAsync();

    /// <summary>
    /// Gets the settings for all vehicles at the provided place.
    /// </summary>
    /// <returns>The list of vehicles.</returns>
    IAsyncEnumerable<VehicleSettings> GetSettingsAsync();

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
    /// Gets the ETag for the settings for the provided place.
    /// </summary>
    /// <returns>An ETag representing the current data.</returns>
    Task<StringSegment> GetSettingsEtagAsync();

    /// <summary>
    /// Gets the ETag for the setting for the provided vehicle.
    /// </summary>
    /// <param name="id">The vehicle's ID.</param>
    /// <returns>An ETag representing the current data.</returns>
    Task<StringSegment> GetSettingsEtagAsync(int id);

    /// <summary>
    /// Gets a report on the national vehicles.
    /// </summary>
    /// <returns>
    /// The national vehicle report.
    /// </returns>
    Task<NationalVehicleReport> GetVehicleReportAsync();

    /// <summary>
    /// Gets the ETag for the national vehicle report.
    /// </summary>
    /// <returns>An ETag representing the current data.</returns>
    Task<StringSegment> GetVehicleReportEtagAsync();

    /// <summary>
    /// Gets the VOR statuses for the provided place.
    /// </summary>
    /// <param name="place">The place to search for.</param>
    /// <returns>The list of statuses.</returns>
    IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Region region);

    /// <summary>
    /// Gets the ETag for the VOR statuses for the provided place.
    /// </summary>
    /// <param name="place">The place.</param>
    /// <returns>An ETag representing the current data.</returns>
    Task<StringSegment> GetVorStatusesEtagAsync(Region region);

    /// <summary>
    /// Updates or creates the settings for the provided vehicle.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PutSettingsAsync(UpdateVehicleSettings settings);
}
