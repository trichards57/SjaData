using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Model.Patient;
using System.Text.Json.Serialization;

namespace SjaData.Server.Model;

[JsonSerializable(typeof(NewHoursEntry))]
[JsonSerializable(typeof(HoursQuery))]
[JsonSerializable(typeof(HoursCount))]
[JsonSerializable(typeof(NewPatient))]
[JsonSerializable(typeof(PatientQuery))]
[JsonSerializable(typeof(PatientCount))]
[JsonSerializable(typeof(EventType))]
[JsonSerializable(typeof(Region))]
[JsonSerializable(typeof(Trust))]
public partial class JsonContext : JsonSerializerContext
{
}
