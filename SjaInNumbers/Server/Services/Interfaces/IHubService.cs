using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IHubService
{
    IAsyncEnumerable<HubSummary> GetAllAsync();
    Task<string> GetAllEtagAsync();
    Task<DateTimeOffset> GetLastModifiedAsync();
    Task<HubName?> GetNameAsync(int id);
    Task<bool> SetNameAsync(int id, HubName hubName);
}
