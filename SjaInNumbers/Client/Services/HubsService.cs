// <copyright file="HubsService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model.Hubs;
using System.Net.Http.Json;

namespace SjaInNumbers.Client.Services;

public class HubsService(HttpClient httpClient) : IHubService
{
    private readonly HttpClient httpClient = httpClient;

    public IAsyncEnumerable<HubSummary> GetHubSummariesAsync()
        => httpClient.GetFromJsonAsAsyncEnumerable<HubSummary>("/api/hubs");

    public async Task<string> GetHubNameAsync(int id)
        => (await httpClient.GetFromJsonAsync<HubName>($"/api/hubs/{id}/name")).Name;

    public Task PostHubNameAsync(int id, string name)
        => httpClient.PostAsJsonAsync($"/api/hubs/{id}/name", new HubName { Name = name });

    public Task PostHubAsync(int districtId, string name)
        => httpClient.PostAsJsonAsync("/api/hubs", new NewHub { DistrictId = districtId, Name = name });
}
