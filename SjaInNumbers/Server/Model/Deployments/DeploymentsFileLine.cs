namespace SjaInNumbers.Server.Model.Deployments;

public class DeploymentsFileLine
{
    public string Id { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Name { get; set; }
    public int? DipsNumber { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly FinishTime { get; set; }
    public string District { get; set; }
    public string ApprovalStage { get; set; }
    public int Ambulances { get; set; }
    public int BlueLightEac { get; set; }
    public int Eacs { get; set; }
    public int Paramedics { get; set; }
    public bool ShiftsCreated { get; set; }
    public string TypeOfEvent { get; set; }
    public string EventLeadResponsible { get; set; }
    public string AmbulanceLead { get; set; }
    public string HubLocation { get; set; }
    public int OffRoadAmbulances { get; set; }
    public int AllWheelDriveAmbulances { get; set; }
    public string Notes { get; set; }
    public string Requester { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateOnly DateAcceptedByLead { get; set; }
    public DateTime Modified { get; set; }
    public string ItemType { get; set; }
    public string Path { get; set; }
}
