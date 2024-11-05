namespace SjaInNumbers.Client.Maths;

public class MonteCarloVehicle(int districtId, double failureProbability, WeibullGenerator repairTimeGenerator, Random random)
{
    private readonly double failureProbability = failureProbability;
    private readonly Random random = random;
    private readonly WeibullGenerator repairTimeGenerator = repairTimeGenerator;
    private int daysToReturn;

    public int DistrictId { get; } = districtId;

    public bool IsAvailable { get; private set; }

    public void Update()
    {
        if (IsAvailable)
        {
            var fail = random.NextDouble() > failureProbability;

            if (fail)
            {
                IsAvailable = false;
                daysToReturn = repairTimeGenerator.Generate();
            }
        }
        else
        {
            if (daysToReturn == 0)
            {
                IsAvailable = true;
            }
        }
    }
}
