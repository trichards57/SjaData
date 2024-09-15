// <copyright file="Patient.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model;
using System.ComponentModel.DataAnnotations;

namespace SjaData.Server.Data;

/// <summary>
/// Represents a patient.
/// </summary>
public class Patient
{
    /// <summary>
    /// Gets or sets the unique identifier for the patient record.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the date of the patient record.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the call sign of the crew that attended the patient.
    /// </summary>
    [StringLength(10)]
    [Required(AllowEmptyStrings = false)]
    public string CallSign { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of event that the patient was involved in.
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// Gets or sets the region that the patient was in.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the NHS Ambulance Trust that the patient was attended by.
    /// </summary>
    public Trust Trust { get; set; }

    /// <summary>
    /// Gets or sets the presenting complaint of the patient.
    /// </summary>
    [StringLength(100)]
    public string PresentingComplaint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the final clinical impression of the patient.
    /// </summary>
    [StringLength(100)]
    public string FinalClinicalImpression { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the outcome of the patient.
    /// </summary>
    public Outcome Outcome { get; set; }

    /// <summary>
    /// Gets or sets the date the entry was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date the entry was deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }
}
