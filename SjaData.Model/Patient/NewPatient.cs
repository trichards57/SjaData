// <copyright file="NewPatient.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model.DataTypes;
using SjaData.Model.Validation;
using System.ComponentModel.DataAnnotations;

namespace SjaData.Model.Patient;

/// <summary>
/// Represents a new patient record.
/// </summary>
[RegionOrTrust]
public readonly record struct NewPatient : IRegionAndTrust
{
    /// <summary>
    /// Gets the ID of the new patient.
    /// </summary>
    [GreaterThan(0)]
    [Required]
    [PatientData]
    public required int Id { get; init; }

    /// <summary>
    /// Gets the date of the new patient.
    /// </summary>
    [Required]
    public required DateOnly Date { get; init; }

    /// <summary>
    /// Gets the call sign of the crew that attended the patient.
    /// </summary>
    [StringLength(10)]
    [Required(AllowEmptyStrings = false)]
    public required string CallSign { get; init; }

    /// <summary>
    /// Gets the type of event that the patient was involved in.
    /// </summary>
    [Required]
    [EnumDataType(typeof(EventType))]
    public EventType EventType { get; init; }

    /// <summary>
    /// Gets the region that the patient was in.
    /// </summary>
    [EnumDataType(typeof(Region))]
    public Region Region { get; init; }

    /// <summary>
    /// Gets the NHS Ambulance Trust that the patient was attended by.
    /// </summary>
    [EnumDataType(typeof(Trust))]
    public Trust Trust { get; init; }

    /// <summary>
    /// Gets the presenting complaint of the patient.
    /// </summary>
    [StringLength(100)]
    [PatientData]
    public string? PresentingComplaint { get; init; }

    /// <summary>
    /// Gets the final clinical impression of the patient.
    /// </summary>
    [StringLength(100)]
    [PatientData]
    public string? FinalClinicalImpression { get; init; }

    /// <summary>
    /// Gets the outcome of the patient.
    /// </summary>
    [Required]
    [EnumDataType(typeof(Outcome))]
    public Outcome Outcome { get; init; }
}
