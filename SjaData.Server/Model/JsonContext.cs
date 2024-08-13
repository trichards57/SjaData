using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Data;
using SjaData.Server.Model.Hours;
using SjaData.Server.Model.Patient;
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
[JsonSerializable(typeof(ProblemDetails))]
public partial class JsonContext : JsonSerializerContext
{
}
