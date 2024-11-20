// <copyright file="IDistrictsService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model.Districts;

namespace SjaInNumbers2.Client.Services.Interfaces;

public interface IDistrictsService
{
    Task<DistrictSummary> GetDistrictAsync(int id);

    IAsyncEnumerable<DistrictSummary> GetDistrictSummariesAsync();

    Task PostDistrictCode(int id, string code);

    Task PostDistrictMerge(int sourceId, int destinationId);

    Task PostDistrictName(int id, string name);
}
