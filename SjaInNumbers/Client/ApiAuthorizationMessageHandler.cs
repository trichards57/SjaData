// <copyright file="ApiAuthorizationMessageHandler.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SjaInNumbers.Client;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager, IConfiguration configuration)
        : base(provider, navigationManager)
    {
        ConfigureHandler(authorizedUrls: [configuration.GetValue("ApiBase", "https://localhost:7191/")]);
    }
}
