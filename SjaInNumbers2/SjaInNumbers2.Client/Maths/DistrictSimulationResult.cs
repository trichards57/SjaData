namespace SjaInNumbers2.Client.Maths;

public readonly record struct DistrictSimulationResult
{
    public int DaysShort { get; init; }

    public double DaysShortStandardDeviation { get; init; }

    public int TotalMoves { get; init; }

    public double TotalMovesStandardDeviation { get; init; }
}
