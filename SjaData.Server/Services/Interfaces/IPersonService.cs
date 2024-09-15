﻿// <copyright file="IPersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model;
using SjaData.Server.Model.People;

namespace SjaData.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing people.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Adds people to the database.
    /// </summary>
    /// <param name="people">The people to add.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the number of people added or updated.
    /// </returns>
    Task<int> AddPeople(IAsyncEnumerable<PersonFileLine> people);
    IAsyncEnumerable<PersonReport> GetPeopleReports(Region region);
}
