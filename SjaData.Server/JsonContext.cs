// <copyright file="JsonContext.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Model.Hours;
using SjaData.Server.Model.Patient;
using System.Text.Json.Serialization;

namespace SjaData.Server.Model;

/// <summary>
/// Context for JSON serialization.
/// </summary>
[JsonSerializable(typeof(HoursTarget))]
[JsonSerializable(typeof(HoursCount))]
[JsonSerializable(typeof(PatientCount))]
[JsonSerializable(typeof(EventType))]
[JsonSerializable(typeof(Region))]
[JsonSerializable(typeof(Trust))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(HttpValidationProblemDetails))]
public partial class JsonContext : JsonSerializerContext
{
}
