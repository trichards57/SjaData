namespace SjaInNumbers.Client.Maths;

public readonly record struct SimulationResult
{
    public Dictionary<int, DistrictSimulationResult> DistrictResults { get; init; }
    public double AverageAvailability { get; init; }
}
