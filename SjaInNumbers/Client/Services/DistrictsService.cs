using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model.Districts;
using System.Net.Http.Json;

namespace SjaInNumbers.Client.Services;

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
}
