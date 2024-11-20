// <copyright file="HubsService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model.Hubs;
using SjaInNumbers2.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SjaInNumbers2.Client.Services;

public class HubsService(HttpClient httpClient) : IHubService
{
    private readonly HttpClient httpClient = httpClient;

    public IAsyncEnumerable<HubSummary> GetHubSummariesAsync()
        => httpClient.GetFromJsonAsAsyncEnumerable<HubSummary>("/api/hubs");

    public async Task<HubName> GetHubNameAsync(int id)
        => await httpClient.GetFromJsonAsync<HubName>($"/api/hubs/{id}/name");

    public Task PostHubNameAsync(int id, HubName name)
        => httpClient.PostAsJsonAsync($"/api/hubs/{id}/name", name);

    public Task PostHubAsync(NewHub hub)
        => httpClient.PostAsJsonAsync("/api/hubs", hub);
}
