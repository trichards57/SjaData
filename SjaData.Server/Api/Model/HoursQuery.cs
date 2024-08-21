// <copyright file="HoursQuery.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model;
using System.Globalization;

namespace SjaData.Server.Api.Model;

public readonly record struct HoursQuery
{
    public DateOnly? Date { get; init; }

    public DateType? DateType { get; init; }

    public static ValueTask<HoursQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(new HoursQuery
        {
            Date = context.Request.Query.TryGetValue("date", out var date) ? DateOnly.Parse(date!, CultureInfo.InvariantCulture) : null,
            DateType = context.Request.Query.TryGetValue("dateType", out var dateType) ? Enum.Parse<DateType>(dateType!) : null,
        });
    }
}
