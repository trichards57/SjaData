// <copyright file="PatientQuery.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model;
using SjaData.Model.Converters;
using System.Globalization;

namespace SjaData.Server.Api.Model;

/// <summary>
/// Represents a query for patient data.
/// </summary>
public readonly record struct PatientQuery
{
    /// <summary>
    /// Gets the region to filter by.
    /// </summary>
    public Region? Region { get; init; }

    /// <summary>
    /// Gets the trust to filter by.
    /// </summary>
    public Trust? Trust { get; init; }

    /// <summary>
    /// Gets the event type to filter by.
    /// </summary>
    public EventType? EventType { get; init; }

    /// <summary>
    /// Gets the patient outcome to filter by.
    /// </summary>
    public Outcome? Outcome { get; init; }

    /// <summary>
    /// Gets the date to filter by.
    /// </summary>
    public DateOnly? Date { get; init; }

    /// <summary>
    /// Gets the interpretation of <see cref="Date" /> for filtering.
    /// </summary>
    public DateType? DateType { get; init; }

    /// <summary>
    /// Binds a <see cref="PatientQuery"/> from the given <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">The request's <see cref="HttpContext"/>.</param>
    /// <returns>
    /// The new <see cref="PatientQuery"/>.
    /// </returns>
    public static ValueTask<PatientQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(new PatientQuery
        {
            Date = context.Request.Query.TryGetValue("date", out var date) ? DateOnly.Parse(date!, CultureInfo.InvariantCulture) : null,
            DateType = context.Request.Query.TryGetValue("dateType", out var dateType) ? Enum.Parse<DateType>(dateType!, true) : null,
            EventType = context.Request.Query.TryGetValue("eventType", out var eventType) ? Enum.Parse<EventType>(eventType!, true) : null,
            Outcome = context.Request.Query.TryGetValue("outcome", out var outcome) ? Enum.Parse<Outcome>(outcome!, true) : null,
            Region = context.Request.Query.TryGetValue("region", out var region) ? RegionConverter.FromString(region!) : null,
            Trust = context.Request.Query.TryGetValue("trust", out var trust) ? TrustConverter.FromString(trust!) : null,
        });
    }
}
