// <copyright file="IPatientService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model;
using SjaData.Server.Model.Patient;

namespace SjaData.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing patients.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Counts the patients that match the given query.
    /// </summary>
    /// <param name="region">The region to filter by.</param>
    /// <param name="trust">The trust to filter by.</param>
    /// <param name="eventType">The event type to filter by.</param>
    /// <param name="outcome">The patient outcome to filter by.</param>
    /// <param name="date">The date to filter by.</param>
    /// <param name="dateType">The level the date should be filtered with.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the count.
    /// </returns>
    Task<PatientCount> CountAsync(Region? region, Trust? trust, EventType? eventType, Outcome? outcome, DateOnly? date, DateType? dateType);

    /// <summary>
    /// Deletes the given patients entry.
    /// </summary>
    /// <param name="id">The ID of the entry to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteAsync(int id);

    /// <summary>
    /// Gets the date the patient data was last modified.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the date.
    /// </returns>
    Task<DateTimeOffset> GetLastModifiedAsync();
}
