// <copyright file="UpdateAvailableDetector.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.JSInterop;

namespace SjaInNumbers.Client.Components;

public partial class UpdateAvailableDetector
{
    private bool newVersionAvailable = false;

    [JSInvokable(nameof(OnUpdateAvailable))]
    public Task OnUpdateAvailable()
    {
        newVersionAvailable = true;

        StateHasChanged();

        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        await RegisterForUpdateAvailableNotification();
    }

    private async Task RegisterForUpdateAvailableNotification()
    {
        await JSRuntime.InvokeAsync<object>(
            identifier: "registerForUpdateAvailableNotification",
            DotNetObjectReference.Create(this),
            nameof(OnUpdateAvailable));
    }
}
