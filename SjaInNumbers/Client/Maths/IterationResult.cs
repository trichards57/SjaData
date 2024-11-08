namespace SjaInNumbers.Client.Maths;

public readonly record struct IterationResult
{
    public int DaysWithShortages { get; init; }
    public int TotalMoves { get; init; }
}
