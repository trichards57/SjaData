namespace SjaInNumbers.Shared.Model.Hubs;

public readonly record struct NationalHubSummary : IDateMarked
{
    public IEnumerable<HubSummary> Hubs { get; init; }
    public string ETag { get; init; }
    public DateTimeOffset LastModified { get; init; }
}
