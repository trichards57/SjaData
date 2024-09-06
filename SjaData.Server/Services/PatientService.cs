// <copyright file="PatientService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Model;
using SjaData.Model.Converters;
using SjaData.Model.Patient;
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
    public async Task<PatientCount> CountAsync(Region? region, Trust? trust, EventType? eventType, Outcome? outcome, DateOnly? date, DateType? dateType)
    {
        var items = dataContext.Patients.AsQueryable();

        if (trust.HasValue && trust != Trust.Undefined)
        {
            items = items.Where(p => p.Trust == trust.Value);
        }

        if (region.HasValue && region != Region.Undefined)
        {
            items = items.Where(p => p.Region == region.Value);
        }

        if (date.HasValue)
        {
            items = dateType switch
            {
                DateType.Day => items.Where(p => p.Date == date.Value),
                DateType.Month => items.Where(p => p.Date.Month == date.Value.Month && p.Date.Year == date.Value.Year),
                _ => items.Where(p => p.Date.Year == date.Value.Year),
            };
        }

        if (eventType.HasValue && eventType != EventType.Undefined)
        {
            items = items.Where(p => p.EventType == eventType.Value);
        }

        if (outcome.HasValue && outcome != Outcome.Undefined)
        {
            items = items.Where(p => p.Outcome == outcome.Value);
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
