using CsvHelper.Configuration;
using SjaInNumbers.Server.Model.Hours;

namespace SjaInNumbers.Server.Model.Deployments;

public class DeploymentsFileLineMap : ClassMap<DeploymentsFileLine>
{
    public DeploymentsFileLineMap()
    {
        Map(h => h.Id).Name("AOB ID");
        Map(h => h.Date).Name("EventDate");
        Map(h => h.Name).Name("Event Name");
        Map(h => h.DipsNumber).Name("DIPS number");
        Map(h => h.StartTime).Name("Start Time");
        Map(h => h.FinishTime).Name("Finish Time");
        Map(h => h.District).Name("District");
        Map(h => h.ApprovalStage).Name("Approval stage");
        Map(h => h.Ambulances).Name("Ambulances");
        Map(h => h.BlueLightEac).Name("BL EAC");
        Map(h => h.Eacs).Name("EAC");
        Map(h => h.Paramedics).Name("Paramedics");
        Map(h => h.ShiftsCreated).Name("GRS ShiftCreated");
        Map(h => h.TypeOfEvent).Name("Type of Event");
        Map(h => h.EventLeadResponsible).Name("Event Lead Responsible");
        Map(h => h.AmbulanceLead).Name("Ambulance Lead");
        Map(h => h.HubLocation).Name("Hub Location");
        Map(h => h.OffRoadAmbulances).Name("4x4");
        Map(h => h.AllWheelDriveAmbulances).Name("ORA");
        Map(h => h.Notes).Name("Notes");
        Map(h => h.Requester).Name("Requestor");
        Map(h => h.CreatedAt).Name("Created");
        Map(h => h.DateAcceptedByLead).Name("Date Event accepted by Ambulance Lead");
        Map(h => h.Modified).Name("Modified");
        Map(h => h.ItemType).Name("Item Type");
        Map(h => h.Path).Name("Path");
    }
}
