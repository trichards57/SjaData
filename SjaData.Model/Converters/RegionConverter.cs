using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SjaData.Model.Converters;

public class RegionConverter : JsonConverter<Region>
{
    public static OpenApiSchema Schema =>
        new()
        {
            Title = "Region",
            Type = "string",
            Enum = [
                new OpenApiString("NE"),
                new OpenApiString("NW"),
                new OpenApiString("WM"),
                new OpenApiString("EM"),
                new OpenApiString("EOE"),
                new OpenApiString("LON"),
                new OpenApiString("SE"),
                new OpenApiString("SW"),
            ],
        };

    public static IEnumerable<string> GetNames() => Enum.GetValues<Region>().Where(s => s != Region.Undefined).Select(ToString);

    public static Region FromString(string value) => value switch
    {
        "NE" => Region.NorthEast,
        "NW" => Region.NorthWest,
        "WM" => Region.WestMidlands,
        "EM" => Region.EastMidlands,
        "EOE" => Region.EastOfEngland,
        "LON" => Region.London,
        "SE" => Region.SouthEast,
        "SW" => Region.SouthWest,
        _ => Region.Undefined,
    };

    public static string ToString(Region value) => value switch
    {
        Region.NorthEast => "NE",
        Region.NorthWest => "NW",
        Region.WestMidlands => "WM",
        Region.EastMidlands => "EM",
        Region.EastOfEngland => "EOE",
        Region.London => "LON",
        Region.SouthEast => "SE",
        Region.SouthWest => "SW",
        _ => string.Empty,
    };

    public override Region Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        => FromString(reader.GetString() ?? string.Empty);

    public override void Write(Utf8JsonWriter writer, Region value, JsonSerializerOptions options)
        => writer.WriteStringValue(ToString(value));
    
}
