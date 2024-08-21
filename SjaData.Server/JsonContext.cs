using Microsoft.AspNetCore.Mvc;
using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Model.Patient;
using System.Text.Json.Serialization;

namespace SjaData.Server.Model;

[JsonSerializable(typeof(NewHoursEntry))]
[JsonSerializable(typeof(HoursTarget))]
[JsonSerializable(typeof(HoursCount))]
[JsonSerializable(typeof(NewPatient))]
[JsonSerializable(typeof(PatientCount))]
[JsonSerializable(typeof(EventType))]
[JsonSerializable(typeof(Region))]
[JsonSerializable(typeof(Trust))]
[JsonSerializable(typeof(ProblemDetails))]
public partial class JsonContext : JsonSerializerContext
{
}
