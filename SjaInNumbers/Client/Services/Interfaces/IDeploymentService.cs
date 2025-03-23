// <copyright file="IDeploymentService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IDeploymentService
{
    Task<NationalSummary> GetNationalSummary();

    IAsyncEnumerable<PeakLoads> GetPeakLoads();
}
