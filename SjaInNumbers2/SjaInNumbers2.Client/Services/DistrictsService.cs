// <copyright file="DistrictsService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model.Districts;
using SjaInNumbers2.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SjaInNumbers2.Client.Services;

public class DistrictsService(HttpClient httpClient) : IDistrictsService
{
    private readonly HttpClient httpClient = httpClient;

    public IAsyncEnumerable<DistrictSummary> GetDistrictSummariesAsync()
        => httpClient.GetFromJsonAsAsyncEnumerable<DistrictSummary>("/api/districts");

    public Task<DistrictSummary> GetDistrictAsync(int id)
        => httpClient.GetFromJsonAsync<DistrictSummary>($"/api/districts/{id}");

    public Task PostDistrictCode(int id, string code)
        => httpClient.PostAsJsonAsync($"/api/districts/{id}/code", code);

    public Task PostDistrictName(int id, string name)
        => httpClient.PostAsJsonAsync($"/api/districts/{id}/name", name);

    public Task PostDistrictMerge(int sourceId, int destinationId)
        => httpClient.PostAsJsonAsync($"/api/districts/merge", new MergeDistrict { SourceDistrictId = sourceId, DestinationDistrictId = destinationId });
}
