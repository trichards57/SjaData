// <copyright file="PersonService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.WebUtilities;
using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model;
using SjaInNumbers.Shared.Model.People;
using System.Net.Http.Json;

namespace SjaInNumbers.Client.Services;

public class PersonService(HttpClient httpClient) : IPersonService
{
    private readonly HttpClient httpClient = httpClient;

    public IAsyncEnumerable<PersonReport> GetPeopleReportsAsync(DateOnly date, Region region)
    {
        var uri = QueryHelpers.AddQueryString(
           "/api/people/reports",
           new Dictionary<string, string?>()
           {
                { "date", date.ToString("o") },
                { "region", region.ToString() },
           });

        return httpClient.GetFromJsonAsAsyncEnumerable<PersonReport>(uri);
    }
}
