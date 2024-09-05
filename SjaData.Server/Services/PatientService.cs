// <copyright file="PatientService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Model;
using SjaData.Model.Converters;
using SjaData.Model.Patient;
using SjaData.Server.Api.Model;
using SjaData.Server.Data;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

/// <summary>
/// A service for managing patients.
/// </summary>
/// <param name="dataContext">The data context containing the patient data.</param>
/// <param name="logger">The logger to write to.</param>
public partial class PatientService(DataContext dataContext, ILogger<PatientService> logger) : IPatientService
{
    private readonly DataContext dataContext = dataContext;
    private readonly ILogger<PatientService> logger = logger;

    /// <inheritdoc/>
    public async Task AddAsync(NewPatient patient)
    {
        var existingItem = await dataContext.Patients.FirstOrDefaultAsync(p => p.Id == patient.Id);
        var newItem = false;

        if (existingItem is null)
        {
            newItem = true;
            existingItem = new Patient();
            dataContext.Patients.Add(existingItem);
            existingItem.Id = patient.Id;
        }

        existingItem.CallSign = patient.CallSign;
        existingItem.Date = patient.Date;
        existingItem.EventType = patient.EventType;
        existingItem.FinalClinicalImpression = patient.FinalClinicalImpression ?? string.Empty;
        existingItem.Outcome = patient.Outcome;
        existingItem.PresentingComplaint = patient.PresentingComplaint ?? string.Empty;
        existingItem.Region = patient.Region;
        existingItem.Trust = patient.Trust;
        existingItem.CreatedAt = DateTimeOffset.UtcNow;
        existingItem.DeletedAt = null;

        dataContext.Add(existingItem);

        await dataContext.SaveChangesAsync();

        if (newItem)
        {
            LogItemCreated(patient);
        }
        else
        {
            LogItemModified(existingItem.Id, patient);
        }
    }

    /// <inheritdoc/>
    public async Task<DateTimeOffset> GetLastModifiedAsync()
    {
        if (await dataContext.Patients.AnyAsync())
        {
            return await dataContext.Patients.MaxAsync(p => p.DeletedAt ?? p.CreatedAt);
        }

        return DateTimeOffset.MinValue;
    }

    /// <inheritdoc/>
    public async Task<PatientCount> CountAsync(PatientQuery query)
    {
        var items = dataContext.Patients.AsQueryable();

        if (query.Trust.HasValue && query.Trust != Trust.Undefined)
        {
            items = items.Where(p => p.Trust == query.Trust.Value);
        }

        if (query.Region.HasValue && query.Region != Region.Undefined)
        {
            items = items.Where(p => p.Region == query.Region.Value);
        }

        if (query.Date.HasValue)
        {
            items = query.DateType switch
            {
                DateType.Day => items.Where(p => p.Date == query.Date.Value),
                DateType.Month => items.Where(p => p.Date.Month == query.Date.Value.Month && p.Date.Year == query.Date.Value.Year),
                _ => items.Where(p => p.Date.Year == query.Date.Value.Year),
            };
        }

        if (query.EventType.HasValue && query.EventType != EventType.Undefined)
        {
            items = items.Where(p => p.EventType == query.EventType.Value);
        }

        if (query.Outcome.HasValue && query.Outcome != Outcome.Undefined)
        {
            items = items.Where(p => p.Outcome == query.Outcome.Value);
        }

        var count = (await items.Where(i => i.DeletedAt == null).Select(h => new
        {
            h.Region,
            h.Trust,
        }).ToListAsync())
        .Select(h => new
        {
            Label = h.Region == Region.Undefined ? TrustConverter.ToString(h.Trust) : RegionConverter.ToString(h.Region),
        })
        .GroupBy(h => h.Label)
        .ToDictionary(h => h.Key, h => h.Count());

        var lastUpdate = await GetLastModifiedAsync();

        return new PatientCount { Counts = new AreaDictionary<int>(count), LastUpdate = lastUpdate };
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id)
    {
        var existingItem = await dataContext.Patients.FirstOrDefaultAsync(h => h.Id == id && !h.DeletedAt.HasValue);

        if (existingItem is not null)
        {
            existingItem.DeletedAt = DateTimeOffset.UtcNow;
            await dataContext.SaveChangesAsync();
            LogItemDeleted(id);
        }
    }

    [LoggerMessage(EventCodes.ItemDeleted, LogLevel.Information, "Patient entry {id} has been deleted.")]
    private partial void LogItemDeleted(int id);

    [LoggerMessage(EventCodes.ItemCreated, LogLevel.Information, "Patient entry has been created.")]
    private partial void LogItemCreated([LogProperties] NewPatient hours);

    [LoggerMessage(EventCodes.ItemModified, LogLevel.Information, "Patient entry {id} has been updated.")]
    private partial void LogItemModified(int id, [LogProperties] NewPatient hours);
}
