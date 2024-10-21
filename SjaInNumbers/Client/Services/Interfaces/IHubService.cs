using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IHubService
{
    Task<HubName> GetHubNameAsync(int id);

    Task PostHubNameAsync(int id, HubName name);

    IAsyncEnumerable<HubSummary> GetHubSummariesAsync();
}
