namespace SjaData.Model.Patient;

public readonly record struct PatientCount
{
    public int Count { get; init; }
    public DateTimeOffset LastUpdate { get; init; }
}
