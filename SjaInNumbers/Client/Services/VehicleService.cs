// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Vehicles;
using System.Net.Http.Json;

namespace SjaInNumbers.Client.Services;

public class VehicleService(HttpClient client) : IVehicleService
{
    private readonly HttpClient client = client;

    public IAsyncEnumerable<VehicleSettings> GetVehicleSettings()
        => client.GetFromJsonAsAsyncEnumerable<VehicleSettings>("/api/vehicles");

    public async Task<VehicleSettings> GetVehicleSettingsAsync(int id) => await client.GetFromJsonAsync<VehicleSettings>($"/api/vehicles/{id}");

    public async Task PostVehicleSettingsAsync(UpdateVehicleSettings settings) => await client.PostAsJsonAsync($"/api/vehicles", settings);

    public IAsyncEnumerable<VorStatus> GetVorStatus(Region region)
        => client.GetFromJsonAsAsyncEnumerable<VorStatus>($"/api/vor?region={region}");
}
