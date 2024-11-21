// <copyright file="IDistrictsService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model;
using SjaInNumbers2.Client.Model.Districts;

namespace SjaInNumbers2.Client.Services.Interfaces;

public interface IDistrictsService
{
    Task<DistrictSummary?> GetDistrictAsync(int id);

    IAsyncEnumerable<DistrictSummary> GetDistrictSummariesAsync();

    Task<bool> SetDistrictCodeAsync(int id, string code);

    Task<bool> MergeDistrictsAsync(MergeDistrict mergeDistrict);

    Task<bool> SetDistrictNameAsync(int id, string name);

    Task<int?> GetIdByNameAsync(string name, Region region);

    Task<DistrictSummary?> GetIdByDistrictCodeAsync(string district);
}
