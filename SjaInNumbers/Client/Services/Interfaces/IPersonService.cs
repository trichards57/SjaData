﻿// <copyright file="IPersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.People;

namespace SjaInNumbers.Client.Services.Interfaces;

/// <summary>
/// Represents a service for managing people.
/// </summary>
internal interface IPersonService
{
    /// <summary>
    /// Gets the people activity reports for a specific end-date and region.
    /// </summary>
    /// <param name="date">The date of the last activity.</param>
    /// <param name="region">The region to report on.</param>
    /// <returns>
    /// The people activity reports.
    /// </returns>
    IAsyncEnumerable<PersonReport> GetPeopleReportsAsync(DateOnly date, Region region);
}
