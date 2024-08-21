// See https://aka.ms/new-console-template for more information
using CsvHelper;
using SjaData.Model.Hours;
using SjaData.Server.Data;
using SjaData.Server.Model;
using SJAData.Loader.Model;
using System.Formats.Tar;
using System.Globalization;
using System.Net.Http.Json;

Console.WriteLine("Hello, World!");

if (args.Length == 0)
{
    Console.WriteLine("Please specify an input file name.");
    return -1;
}

var inputFileName = args[0];

if (!File.Exists(inputFileName))
{
    Console.WriteLine("The specified file does not exist.");
    return -1;
}

var hours = new List<Hours>();

using (var reader = new StreamReader(inputFileName))
using (var csv = new CsvReader(reader, CultureInfo.CurrentUICulture))
{
    csv.Context.RegisterClassMap<HoursMap>();
    hours.AddRange(csv.GetRecords<Hours>());
}

foreach (var hour in hours.Where(h => !string.IsNullOrWhiteSpace(h.Name)))
{
    var newEntry = new NewHoursEntry
    {
        Date = hour.ShiftDate,
        Hours = hour.ShiftLength,
        PersonId = int.Parse(hour.IdNumber),
        Name = hour.Name
    };

    var trust = hour.CrewType switch
    {
        "NHS E EEAST" => Trust.EastOfEnglandAmbulanceService,
        "NHS E EMAS" => Trust.EastOfEnglandAmbulanceService,
        "NHS E IOW" => Trust.IsleOfWightAmbulanceService,
        "NHS E LAS" => Trust.LondonAmbulanceService,
        "NHS E NEAS" => Trust.NorthEastAmbulanceService,
        "NHS E NWAS" => Trust.NorthWestAmbulanceService,
        "NWAS 365" => Trust.NorthWestAmbulanceService,
        "NHS E SCAS" => Trust.SouthCentralAmbulanceService,
        "NHS E SECAmb" => Trust.SouthEastCoastAmbulanceService,
        "NHS E SWAST" => Trust.SouthWesternAmbulanceService,
        "NHS E YAS" => Trust.YorkshireAmbulanceService,
        "YAS" => Trust.YorkshireAmbulanceService,
        _ => Trust.Undefined,
    };

    if (trust == Trust.Undefined)
    {
        continue;
    }

    newEntry = newEntry with { Trust = trust };

    HttpClient client = new();

    await client.PostAsJsonAsync("https://localhost:7125/api/hours", newEntry, JsonContext.Default.NewHoursEntry);
}

return 0;
