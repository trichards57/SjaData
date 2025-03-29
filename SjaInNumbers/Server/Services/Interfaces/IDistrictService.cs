// <copyright file="IDistrictService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing districts.
/// </summary>
public interface IDistrictService
{
    /// <summary>
    /// Gets all of the registered districts.
    /// </summary>
    /// <returns>The list of districts.</returns>
    IAsyncEnumerable<DistrictSummary> GetAll();

    /// <summary>
    /// Gets the summary view of the given district.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <returns>
    /// A summary view of the district, or <see langword="null"/> if the district does not exist.
    /// </returns>
    Task<DistrictSummary> GetDistrict(int id);

    /// <summary>
    /// Gets the ID of a district using it's code.
    /// </summary>
    /// <param name="code">The district code.</param>
    /// <returns>
    /// The ID of the district, or <see langword="null"/> if the district does not exist.
    /// </returns>
    Task<int?> GetIdByDistrictCodeAsync(string code);

    /// <summary>
    /// Gets the ID of a district using it's name, including any previous names.
    /// </summary>
    /// <param name="name">The name of the district.</param>
    /// <param name="region">The region of the district.</param>
    /// <returns>
    /// The ID of the district, or <see langword="null"/> if the district does not exist.
    /// </returns>
    Task<int?> GetIdByNameAsync(string name, Region region);

    /// <summary>
    /// Merges two districts by moving all items from one source to destination to another and then removing the source.
    /// </summary>
    /// <param name="mergeDistrict">The districts to merge.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous action.
    /// </returns>
    Task MergeDistrictsAsync(MergeDistrict mergeDistrict);

    /// <summary>
    /// Sets the district code for the given district.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <param name="code">The code to set.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous action.
    /// </returns>
    Task SetDistrictCodeAsync(int id, string code);

    /// <summary>
    /// Sets the district name for the given district.
    /// </summary>
    /// <param name="id">The ID of the district.</param>
    /// <param name="name">The name to set.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous action.
    /// </returns>
    Task SetDistrictNameAsync(int id, string name);
}
