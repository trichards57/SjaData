namespace SjaData.Server.Model.People;

public class PersonReport
{
    public string Name { get; set; }
    public int MonthsSinceLastActive { get; set; }
    public double[] Hours { get; set; }
    public double HoursThisYear { get; set; }
}
