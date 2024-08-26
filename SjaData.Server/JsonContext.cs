// <copyright file="JsonContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Model.Patient;
using System.Text.Json.Serialization;

namespace SjaData.Server.Model;

/// <summary>
/// Context for JSON serialization.
/// </summary>
[JsonSerializable(typeof(NewHoursEntry))]
[JsonSerializable(typeof(HoursTarget))]
[JsonSerializable(typeof(HoursCount))]
[JsonSerializable(typeof(NewPatient))]
[JsonSerializable(typeof(PatientCount))]
[JsonSerializable(typeof(EventType))]
[JsonSerializable(typeof(Region))]
[JsonSerializable(typeof(Trust))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(HttpValidationProblemDetails))]
public partial class JsonContext : JsonSerializerContext
{
}
