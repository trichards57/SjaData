namespace SjaData.Server.Model.Hours;

public readonly record struct HoursCount
{
    public TimeSpan Count { get; init; }
    public DateTimeOffset LastUpdate { get; init; }
}
