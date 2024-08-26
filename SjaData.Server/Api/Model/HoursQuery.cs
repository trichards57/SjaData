// <copyright file="HoursQuery.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model;
using System.Globalization;

namespace SjaData.Server.Api.Model;

/// <summary>
/// Represents a query for hours entries.
/// </summary>
public readonly record struct HoursQuery
{
    /// <summary>
    /// Gets the date to filter by.
    /// </summary>
    public DateOnly? Date { get; init; }

    /// <summary>
    /// Gets the intepretation of <see cref="Date"/> for querying.
    /// </summary>
    public DateType? DateType { get; init; }

    /// <summary>
    /// Binds a <see cref="HoursQuery"/> from the given <see cref="HttpContext"/>.
    /// </summary>
    /// <param name="context">The request's <see cref="HttpContext"/>.</param>
    /// <returns>
    /// The new <see cref="HoursQuery"/>.
    /// </returns>
    public static ValueTask<HoursQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(new HoursQuery
        {
            Date = context.Request.Query.TryGetValue("date", out var date) ? DateOnly.Parse(date!, CultureInfo.InvariantCulture) : null,
            DateType = context.Request.Query.TryGetValue("dateType", out var dateType) ? Enum.Parse<DateType>(dateType!, true) : null,
        });
    }
}
