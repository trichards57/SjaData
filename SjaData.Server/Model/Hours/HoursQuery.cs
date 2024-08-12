// <copyright file="HoursQuery.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Data;

namespace SjaData.Server.Model.Hours;

public readonly record struct HoursQuery
{
    public static ValueTask<HoursQuery> BindAsync(HttpContext context)
    {
        return ValueTask.FromResult(new HoursQuery
        {
            Date = context.Request.Query.TryGetValue("date", out var date) ? DateOnly.Parse(date) : null,
            DateType = context.Request.Query.TryGetValue("dateType", out var dateType) ? Enum.Parse<DateType>(dateType) : null,
            Region = context.Request.Query.TryGetValue("region", out var region) ? Enum.Parse<Region>(region) : null,
            Trust = context.Request.Query.TryGetValue("trust", out var trust) ? Enum.Parse<Trust>(trust) : null,
        });
    }

    public Region? Region { get; init; }

    public Trust? Trust { get; init; }

    public DateOnly? Date { get; init; }

    public DateType? DateType { get; init; }
}
