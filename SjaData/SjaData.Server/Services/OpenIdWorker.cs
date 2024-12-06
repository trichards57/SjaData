// -----------------------------------------------------------------------
// <copyright file="OpenIdWorker.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SjaData.Server.Services;

/// <summary>
/// A worker that sets up the Open ID server.
/// </summary>
/// <param name="serviceProvider">The core service provider.</param>
/// <param name="options">The options for the worker.</param>
public class OpenIdWorker(IServiceProvider serviceProvider, IOptions<OpenIdWorkerSettings> options) : IHostedService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly OpenIdWorkerSettings options = options.Value;

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync(options.VorUploaderClientId, cancellationToken) == null)
        {
            await manager.CreateAsync(
                new OpenIddictApplicationDescriptor
                {
                    ClientId = options.VorUploaderClientId,
                    ClientSecret = options.VorUploaderClientSecret,
                    DisplayName = "VOR Uploader",
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.Endpoints.Revocation,
                        "vor:edit",
                    },
                },
                cancellationToken);
        }
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
