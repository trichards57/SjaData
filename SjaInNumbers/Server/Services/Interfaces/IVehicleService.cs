// <copyright file="IVehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IVehicleService
{
    Task AddEntriesAsync(IEnumerable<VorIncident> vorIncidents);

    IAsyncEnumerable<VehicleSettings> GetSettingsAsync(Place place);

    Task<VehicleSettings?> GetSettingsAsync(int id);

    Task<VorStatistics?> GetVorStatisticsAsync(Place place);

    IAsyncEnumerable<VorStatus> GetVorStatusesAsync(Place place);

    Task PutSettingsAsync(UpdateVehicleSettings settings);
}
