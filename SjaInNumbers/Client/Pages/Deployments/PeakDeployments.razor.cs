// <copyright file="PeakDeployments.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Client.Pages.Deployments;

[Authorize(Policy = "Lead")]
public partial class PeakDeployments
{
    private IList<PeakLoads> peaks = [];

    [Inject]
    public required IDeploymentService DeploymentService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        peaks = await DeploymentService.GetPeakLoads().ToListAsync();
    }
}
