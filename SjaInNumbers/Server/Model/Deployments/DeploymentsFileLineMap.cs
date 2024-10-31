using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using SjaInNumbers.Server.Model.Hours;
using System.Globalization;

namespace SjaInNumbers.Server.Model.Deployments;

public class DeploymentsFileLineMap : ClassMap<DeploymentsFileLine>
{
    public DeploymentsFileLineMap()
    {
        Map(h => h.Id).Name("AOB ID");
        Map(h => h.Date).Name("EventDate").TypeConverter<DateOnlyConverter>();
        Map(h => h.Name).Name("Event Name");
        Map(h => h.DipsNumber).Name("DIPS number").TypeConverter<DipsConverter>();
        Map(h => h.StartTime).Name("Start Time").TypeConverter<TimeOnlyConverter>();
        Map(h => h.FinishTime).Name("Finish Time").TypeConverter<TimeOnlyConverter>();
        Map(h => h.District).Name("District");
        Map(h => h.ApprovalStage).Name("Approval stage");
        Map(h => h.Ambulances).Name("Ambulances").TypeConverter<OptionalIntConverter>();
        Map(h => h.BlueLightEac).Name("BL EAC").TypeConverter<OptionalIntConverter>();
        Map(h => h.Eacs).Name("EAC").TypeConverter<OptionalIntConverter>();
        Map(h => h.Paramedics).Name("Paramedics").TypeConverter<OptionalIntConverter>();
        Map(h => h.ShiftsCreated).Name("GRS ShiftCreated");
        Map(h => h.TypeOfEvent).Name("Type of Event");
        Map(h => h.EventLeadResponsible).Name("Event Lead Responsible");
        Map(h => h.AmbulanceLead).Name("Ambulance Lead");
        Map(h => h.HubLocation).Name("Hub Location");
        Map(h => h.OffRoadAmbulances).Name("4x4").TypeConverter<OptionalIntConverter>();
        Map(h => h.AllWheelDriveAmbulances).Name("ORA").TypeConverter<OptionalIntConverter>();
        Map(h => h.Notes).Name("Notes");
        Map(h => h.Requester).Name("Requestor");
        Map(h => h.CreatedAt).Name("Created");
        Map(h => h.DateAcceptedByLead).Name("Date Event accepted by Ambulance Lead").TypeConverter<DateOnlyConverter>();
        Map(h => h.Modified).Name("Modified");
        Map(h => h.ItemType).Name("Item Type");
        Map(h => h.Path).Name("Path");
    }

    private sealed class DateOnlyConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return DateOnly.MinValue;
            }

            return DateOnly.Parse(text, CultureInfo.GetCultureInfo("en-GB"));
        }
    }

    private sealed class TimeOnlyConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return TimeOnly.MinValue;
            }

            var str = text.Replace(".", string.Empty).Replace(":", string.Empty).Replace(";", string.Empty).PadLeft(4, '0').Trim()[..4];

            return TimeOnly.ParseExact(str, "HHmm", CultureInfo.InvariantCulture);
        }
    }

    private sealed class DipsConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            if (text.Contains("DO NOT USE", StringComparison.InvariantCultureIgnoreCase))
            {
                return 0;
            }

            return int.TryParse(text.Split("-", 2, StringSplitOptions.RemoveEmptyEntries)[0], out var dips) ? dips : 0;
        }
    }

    private sealed class OptionalIntConverter : DefaultTypeConverter
    {
        public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            return int.Parse(text);
        }
    }
}
