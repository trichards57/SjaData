// <copyright file="Outcome.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json.Serialization;

namespace SjaData.Server.Model;

/// <summary>
/// Represents the outcome of a patient.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Outcome>))]
public enum Outcome : byte
{
    /// <summary>
    /// Undefined outcome.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// The patient was conveyed to hospital.
    /// </summary>
    Conveyed = 1,

    /// <summary>
    /// The patient was not conveyed to hospital.
    /// </summary>
    NotConveyed = 2,
}
