// <copyright file="DistrictsService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model;
using SjaInNumbers2.Client.Model.Districts;
using SjaInNumbers2.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SjaInNumbers2.Client.Services;

public class DistrictsService(HttpClient httpClient) : IDistrictsService
{
    private readonly HttpClient httpClient = httpClient;

    public IAsyncEnumerable<DistrictSummary> GetDistrictSummariesAsync()
        => httpClient.GetFromJsonAsAsyncEnumerable<DistrictSummary>("/api/districts");

    public async Task<DistrictSummary?> GetDistrictAsync(int id)
        => await httpClient.GetFromJsonAsync<DistrictSummary>($"/api/districts/{id}");

    public async Task<bool> SetDistrictCodeAsync(int id, string code)
    {
        await httpClient.PostAsJsonAsync($"/api/districts/{id}/code", code);
        return true;
    }

    public async Task<bool> SetDistrictNameAsync(int id, string name)
    {
        await httpClient.PostAsJsonAsync($"/api/districts/{id}/name", name);

        return true;
    }

    public async Task<bool> MergeDistrictsAsync(MergeDistrict mergeDistrict)
    {
        await httpClient.PostAsJsonAsync($"/api/districts/merge", mergeDistrict);

        return true;
    }

    public Task<int?> GetIdByNameAsync(string name, Region region)
    {
        throw new NotImplementedException();
    }
}
