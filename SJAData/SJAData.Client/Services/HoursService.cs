// <copyright file="HoursService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.WebUtilities;
using SJAData.Client.Data;
using SJAData.Client.Model;
using SJAData.Client.Model.Hours;
using SJAData.Client.Model.Trends;
using SJAData.Client.Services.Interfaces;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SJAData.Client.Services;

internal class HoursService(HttpClient httpClient) : IHoursService
{
    private readonly HttpClient httpClient = httpClient;

    public async Task<HoursCount> CountAsync(DateOnly? date, DateType? dateType = DateType.Month, bool future = false)
    {
        var uri = QueryHelpers.AddQueryString(
            "/api/hours/count",
            new Dictionary<string, string?>()
            {
                { "date", date?.ToString("o") },
                { "date-type", dateType?.ToString() },
                { "future", future.ToString() },
            });

        return await httpClient.GetFromJsonAsync<HoursCount>(uri);
    }

    //public Task DeleteAsync(int id)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<DateTimeOffset> GetLastModifiedAsync()    
    //{
    //    throw new NotImplementedException();
    //}

    public async Task<int> GetNhseTargetAsync()
    {
        var response = await httpClient.GetFromJsonAsync<HoursTarget>("/api/hours/target");

        return response.Target;
    }

    public async Task<Trends> GetTrendsAsync(Region region, bool nhse)
    {
        var uri = QueryHelpers.AddQueryString(
            "/api/hours/trends",
            new Dictionary<string, string?>()
            {
                { "region", region.ToString() },
                { "nhse", nhse.ToString() },
            });

        return await httpClient.GetFromJsonAsync<Trends>(uri);
    }

    //public Task<string> GetTrendsEtagAsync(Region region, bool nhse)
    //{
    //    throw new NotImplementedException();
    //}
}
