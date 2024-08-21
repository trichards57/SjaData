using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJAData.Loader.Model;
internal class HoursMap : ClassMap<Hours>
{
    public HoursMap()
    {
        Map(h => h.Location).Name("Location");
        Map(h => h.ShiftDate).Name("Shift Date");
        Map(h => h.Shift).Name("Shift");
        Map(h => h.Post).Name("Post");
        Map(h => h.Name).Name("Name");
        Map(h => h.IdNumber).Name("IDNUMBER");
        Map(h => h.CrewType).Name("Crew Type");
        Map(h => h.Callsign).Name("Callsign");
        Map(h => h.Required).Name("Required");
        Map(h => h.Relief).Name("Relief");
        Map(h => h.ShiftLength).Name("Shift Length").TypeConverter<TimeSpanConverter>();
        Map(h => h.CrewName).Name("Crew Name");
        Map(h => h.Remarks).Name("Remarks");
    }

    private class TimeSpanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text.Equals("24:00"))
                return TimeSpan.FromDays(1);

            return TimeSpan.Parse(text);
        }

    }
}
