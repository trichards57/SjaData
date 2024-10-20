using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Server.Services;

public class HubService(ApplicationDbContext context) : IHubService
{
    private readonly ApplicationDbContext context = context;

    /// <inheritdoc/>
    public IAsyncEnumerable<HubSummary> GetAllAsync()
    {
        return context.Hubs.Select(s => new HubSummary
        {
            District = s.District.Name,
            Name = s.Name,
            Id = s.Id,
            Region = s.District.Region,
        }).AsAsyncEnumerable();
    }
}
