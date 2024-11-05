namespace SjaInNumbers.Client.Maths;

public readonly record struct SimulationResult
{
    public int DaysShort { get; init; }

    public double DaysShortStandardDeviation { get; init; }

    public int TotalMoves { get; init; }

    public double TotalMovesStandardDeviation { get; init; }
}
