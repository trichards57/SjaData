// <copyright file="HoursMap.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

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

    private sealed class TimeSpanConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            if (text == null)
            {
                return TimeSpan.Zero;
            }

            if (text.Equals("24:00"))
            {
                return TimeSpan.FromDays(1);
            }

            return TimeSpan.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}
