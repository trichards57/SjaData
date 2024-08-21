using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJAData.Loader.Model;

internal class Hours
{
    public string Location { get; set; }
    public DateOnly ShiftDate { get; set; }
    public string Shift { get; set; }
    public string Post { get; set; }
    public string Name { get; set; }
    public string IdNumber { get; set; }
    public string CrewType { get; set; }
    public string Callsign { get; set; }
    public string Required { get; set; }
    public string Relief { get; set; }
    public TimeSpan ShiftLength { get; set; }
    public string CrewName { get; set; }
    public string Remarks { get; set; }
}
