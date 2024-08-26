// <copyright file="IPatientService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model.Patient;
using SjaData.Server.Api.Model;

namespace SjaData.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing patients.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Adds a new patient entry the system.
    /// </summary>
    /// <param name="patient">The new patient data.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddAsync(NewPatient patient);

    /// <summary>
    /// Counts the patients that match the given query.
    /// </summary>
    /// <param name="query">The query to search for.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the count.
    /// </returns>
    Task<PatientCount> CountAsync(PatientQuery query);

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
