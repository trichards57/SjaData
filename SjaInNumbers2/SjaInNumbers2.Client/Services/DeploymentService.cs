// <copyright file="DeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model.Deployments;
using SjaInNumbers2.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SjaInNumbers2.Client.Services;

public class DeploymentService(HttpClient client) : IDeploymentService
{
    private readonly HttpClient client = client;

    public IAsyncEnumerable<PeakLoads> GetPeakLoads()
    {
        return client.GetFromJsonAsAsyncEnumerable<PeakLoads>("/api/deployments/peaks");
    }

    public Task<NationalSummary> GetNationalSummary()
    {
        return client.GetFromJsonAsync<NationalSummary>("/api/deployments/national");
    }
}
