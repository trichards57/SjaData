// <copyright file="PatientService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Model;
using SjaData.Model.Patient;
using SjaData.Server.Api.Model;
using SjaData.Server.Data;
using SjaData.Server.Services.Exceptions;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

public class PatientService(DataContext dataContext) : IPatientService
{
    private readonly DataContext dataContext = dataContext;

    public async Task AddAsync(NewPatient patient)
    {
        if (await dataContext.Patients.AnyAsync(p => p.Id == patient.Id))
        {
            throw new DuplicateIdException();
        }

        var newPatient = new Patient
        {
            Id = patient.Id,
            CallSign = patient.CallSign,
            Date = patient.Date,
            EventType = patient.EventType,
            FinalClinicalImpression = patient.FinalClinicalImpression ?? string.Empty,
            Outcome = patient.Outcome,
            PresentingComplaint = patient.PresentingComplaint ?? string.Empty,
            Region = patient.Region,
            Trust = patient.Trust,
            CreatedAt = DateTimeOffset.UtcNow,
            DeletedAt = null,
        };

        dataContext.Add(newPatient);

        await dataContext.SaveChangesAsync();
    }

    public async Task<DateTimeOffset> GetLastModifiedAsync() => await dataContext.Patients.MaxAsync(p => p.DeletedAt ?? p.CreatedAt);

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
            Label = h.Region == Region.Undefined ? h.Trust.ToString() : h.Region.ToString(),
        })
        .GroupBy(h => h.Label)
        .ToDictionary(h => h.Key, h => h.Count());

        var lastUpdate = await GetLastModifiedAsync();

        return new PatientCount { Counts = new AreaDictionary<int>(count), LastUpdate = lastUpdate };
    }

    public async Task DeleteAsync(int id)
    {
        var existingItem = await dataContext.Patients.FirstOrDefaultAsync(h => h.Id == id && !h.DeletedAt.HasValue);

        if (existingItem is not null)
        {
            existingItem.DeletedAt = DateTimeOffset.UtcNow;
            await dataContext.SaveChangesAsync();
        }
    }
}
