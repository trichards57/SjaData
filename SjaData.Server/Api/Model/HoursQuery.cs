// <copyright file="HoursQuery.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaData.Model;
using SjaData.Server.Controllers.Binders;

namespace SjaData.Server.Api.Model;

/// <summary>
/// Represents a query for hours records.
/// </summary>
public readonly record struct HoursQuery
{
    /// <summary>
    /// Gets the date to filter by.
    /// </summary>
    [BindProperty(Name = "date")]
    public DateOnly? Date { get; init; }

    /// <summary>
    /// Gets the type of date filter to apply.
    /// </summary>
    [BindProperty(Name = "date-type")]
    [ModelBinder(BinderType = typeof(DateTypeBinder))]
    public DateType? DateType { get; init; }
}
