// <copyright file="VehicleService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model.Vehicles;
using System.Net.Http.Json;

namespace SjaInNumbers.Client.Services;

public class VehicleService(HttpClient client) : IVehicleService
{
    private readonly HttpClient client = client;

    public IAsyncEnumerable<VehicleSettings> GetVehicleSettings()
        => client.GetFromJsonAsAsyncEnumerable<VehicleSettings>("/api/vehicles");
}
