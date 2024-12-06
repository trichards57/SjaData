// -----------------------------------------------------------------------
// <copyright file="ConnectController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SjaData.Server.Controllers.Filters;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller responsible for handling OpenID Connect requests.
/// </summary>
/// <param name="applicationManager">OpenIddict Application Manager to use.</param>
/// <param name="scopeManager">OpenIddict Scope Manager to use.</param>
/// <remarks>Does not have it's own logger because OpenIddict logs.</remarks>
[ApiController]
[Route("/connect")]
public sealed partial class ConnectController(IOpenIddictApplicationManager applicationManager, IOpenIddictScopeManager scopeManager) : ControllerBase
{
    private readonly IOpenIddictApplicationManager applicationManager = applicationManager;
    private readonly IOpenIddictScopeManager scopeManager = scopeManager;

    /// <summary>
    /// Handles token exchange requests.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost("token")]
    [IgnoreAntiforgeryToken]
    [Produces("application/json")]
    [NotCachedFilter]
    public async Task<IActionResult> TokenExchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()!;
        if (request.IsClientCredentialsGrantType())
        {
            var application = await applicationManager.FindByClientIdAsync(request.ClientId!) ?? throw new InvalidOperationException("The application details cannot be found in the database.");

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);
            identity.SetClaim(Claims.Subject, await applicationManager.GetClientIdAsync(application));
            identity.SetClaim(Claims.Name, await applicationManager.GetDisplayNameAsync(application));

            if (await applicationManager.HasPermissionAsync(application, "vor:edit"))
            {
                identity.AddClaim("VorData", "Edit");
            }

            identity.SetScopes(request.GetScopes());
            identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
            identity.SetDestinations(GetDestinations);

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        throw new NotImplementedException("The specified grant type is not implemented.");
    }

    private static IEnumerable<string> GetDestinations(Claim claim) => claim.Type switch
    {
        Claims.Name or Claims.Subject => [Destinations.AccessToken, Destinations.IdentityToken],
        _ => [Destinations.AccessToken],
    };
}
