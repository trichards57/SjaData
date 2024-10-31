using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Shared.Model.Deployments;

public readonly record struct DistrictSummary
{
    public int DistrictId { get; init; }
    public string District { get; init; }
    public Region Region { get; init; }
    public Dictionary<DateOnly, int> FrontLineAmbulances { get; init; }
    public Dictionary<DateOnly, int> AllWheelDriveAmbulances { get; init; }
    public Dictionary<DateOnly, int> OffRoadAmbulances { get; init; }
}
