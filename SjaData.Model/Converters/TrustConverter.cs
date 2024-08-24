using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SjaData.Model.Converters;

public class TrustConverter : JsonConverter<Trust>
{
    public static OpenApiSchema Schema =>
        new()
        {
            Title = "Trust",
            Type = "string",
            Enum = [
                new OpenApiString("NEAS"),
                new OpenApiString("NWAS"),
                new OpenApiString("WMAS"),
                new OpenApiString("EMAS"),
                new OpenApiString("EEAST"),
                new OpenApiString("LAS"),
                new OpenApiString("SECAMB"),
                new OpenApiString("SWAST"),
                new OpenApiString("SCAS"),
                new OpenApiString("YAS"),
                new OpenApiString("WAST"),
                new OpenApiString("SAS"),
                new OpenApiString("NIAS"),
                new OpenApiString("IWAS"),
            ],
        };

    public static Trust FromString(string value) => value switch
    {
        "NEAS" => Trust.NorthEastAmbulanceService,
        "NWAS" => Trust.NorthWestAmbulanceService,
        "WMAS" => Trust.WestMidlandsAmbulanceService,
        "EMAS" => Trust.EastMidlandsAmbulanceService,
        "EEAST" => Trust.EastOfEnglandAmbulanceService,
        "LAS" => Trust.LondonAmbulanceService,
        "SECAMB" => Trust.SouthEastCoastAmbulanceService,
        "SWAST" => Trust.SouthWesternAmbulanceService,
        "SCAS" => Trust.SouthCentralAmbulanceService,
        "YAS" => Trust.YorkshireAmbulanceService,
        "WAST" => Trust.WelshAmbulanceService,
        "SAS" => Trust.ScottishAmbulanceService,
        "NIAS" => Trust.NorthernIrelandAmbulanceService,
        "IWAS" => Trust.IsleOfWightAmbulanceService,
        _ => Trust.Undefined,
    };

    public static string ToString(Trust value) => value switch
    {
        Trust.NorthEastAmbulanceService => "NEAS",
        Trust.NorthWestAmbulanceService => "NWAS",
        Trust.WestMidlandsAmbulanceService => "WMAS",
        Trust.EastMidlandsAmbulanceService => "EMAS",
        Trust.EastOfEnglandAmbulanceService => "EEAST",
        Trust.LondonAmbulanceService => "LAS",
        Trust.SouthEastCoastAmbulanceService => "SECAMB",
        Trust.SouthWesternAmbulanceService => "SWAST",
        Trust.SouthCentralAmbulanceService => "SCAS",
        Trust.YorkshireAmbulanceService => "YAS",
        Trust.WelshAmbulanceService => "WAST",
        Trust.ScottishAmbulanceService => "SAS",
        Trust.NorthernIrelandAmbulanceService => "NIAS",
        Trust.IsleOfWightAmbulanceService => "IWAS",
        _ => string.Empty,
    };

    public override Trust Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        => FromString(reader.GetString() ?? string.Empty);

    public override void Write(Utf8JsonWriter writer, Trust value, JsonSerializerOptions options) 
        => writer.WriteStringValue(ToString(value));
}
