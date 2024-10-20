using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IHubService
{
    IAsyncEnumerable<HubSummary> GetAllAsync();
}
