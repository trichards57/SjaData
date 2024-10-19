// -----------------------------------------------------------------------
// <copyright file="OpenIdWorker.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SjaInNumbers.Server.Services;

public class OpenIdWorker(IServiceProvider serviceProvider, IOptions<OpenIdWorkerSettings> options) : IHostedService
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private readonly OpenIdWorkerSettings options = options.Value;

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

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
