// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Primitives;
using SjaInNumbers2.Client.Model;
using SjaInNumbers2.Client.Model.Vehicles;

namespace SjaInNumbers2.Client.Services.Interfaces;

/// <summary>
/// Represents a service for managing vehicles.
/// </summary>
public interface IVehicleService
{
    Task AddEntriesAsync(List<VorIncident> vorIncidents);
    Task<DateTimeOffset?> GetLastModifiedAsync();
    Task<NationalVehicleReport> GetNationalReportAsync();

    /// <summary>
    /// Gets the national vehicle availability statistics.
    /// </summary>
    /// <returns>The statistics by vehicle make, model and body type.</returns>
    IAsyncEnumerable<VehicleTypeStatus> GetNationalStatus();
    Task<StringSegment> GetNationalVorStatusesEtagAsync();
    Task<StringSegment> GetSettingsEtagAsync();
    Task<StringSegment> GetSettingsEtagAsync(int id);
    Task<object?> GetStatisticsAsync(Place place);
    Task<StringSegment> GetVehicleReportEtagAsync();

    /// <summary>
    /// Gets all of the vehicle settings in the system.
    /// </summary>
    /// <returns>The list of vehicles.</returns>
    IAsyncEnumerable<VehicleSettings> GetVehicleSettings();

    /// <summary>
    /// Gets the settings for a specific vehicle.
    /// </summary>
    /// <param name="id">The ID of the vehicle.</param>
    /// <returns>The vehicle's settings.</returns>
    Task<VehicleSettings?> GetVehicleSettingsAsync(int id);
    Task<StringSegment> GetVorStatisticsEtagAsync(Place place);

    /// <summary>
    /// Gets the VOR status for a specific region.
    /// </summary>
    /// <param name="region">The region to query for.</param>
    /// <returns>The list of statuses.</returns>
    IAsyncEnumerable<VorStatus> GetVorStatus(Region region);
    Task<StringSegment> GetVorStatusesEtagAsync(Region region);

    /// <summary>
    /// Updates the settings for a vehicle.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    /// <returns>A task representing the asynchronous activity.</returns>
    Task PostVehicleSettingsAsync(UpdateVehicleSettings settings);
}
