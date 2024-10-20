using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IHubService
{
    IAsyncEnumerable<HubSummary> GetHubSummariesAsync();
}
