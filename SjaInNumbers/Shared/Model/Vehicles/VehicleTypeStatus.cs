namespace SjaInNumbers.Shared.Model.Vehicles;

public readonly record struct VehicleTypeStatus
{
    public string Make { get; init; }

    public string Model { get; init; }

    public string BodyType { get; init; }

    public int CurrentlyAvailable { get; init; }

    public double AverageTwelveMonthAvailability { get; init; }

    public double AverageTwelveMonthMinusOneAvailability { get; init; }

    public double AverageSixMonthAvailability { get; init; }

    public double AverageSixMonthMinusOneAvailability { get; init; }

    public double AverageThreeMonthAvailability { get; init; }

    public double AverageThreeMonthMinusOneAvailability { get; init; }
}
