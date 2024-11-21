// <copyright file="IDeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model.Deployments;

namespace SjaInNumbers2.Client.Services.Interfaces;

public interface IDeploymentService
{
    Task<NationalSummary> GetNationalSummaryAsync();

    IAsyncEnumerable<PeakLoads> GetPeakLoadsAsync();
}
