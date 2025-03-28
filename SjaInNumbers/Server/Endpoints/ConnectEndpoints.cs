// <copyright file="ConnectEndpoints.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SjaInNumbers.Server.Data;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SjaInNumbers.Server.Endpoints;

public static partial class ConnectEndpoints
{
    public static IEndpointRouteBuilder MapConnectEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("connect")
            .AllowAnonymous();

        group.MapGet("authorize", async Task<Results<SignInHttpResult, ForbidHttpResult, ChallengeHttpResult>> (
            HttpContext context,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictAuthorizationManager authorizationManager,
            IOpenIddictScopeManager scopeManager,
            UserManager<ApplicationUser> userManager) =>
        {
            var request = context.GetOpenIddictServerRequest() ??
                   throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var result = await context.AuthenticateAsync();

            if (result == null || !result.Succeeded || request.HasPromptValue(PromptValues.Login) ||
                (request.MaxAge != null && result.Properties?.IssuedUtc != null && DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value)))
            {
                if (request.HasPromptValue(PromptValues.None))
                {
                    return TypedResults.Forbid(new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user must authenticate to access this resource.",
                    }));
                }

                List<KeyValuePair<string, StringValues>> parameters = context.Request.HasFormContentType ?
                    [.. context.Request.Form.Where(p => p.Key != Parameters.Prompt)] :
                    [.. context.Request.Query.Where(p => p.Key != Parameters.Prompt)];
                return TypedResults.Challenge(new AuthenticationProperties { RedirectUri = context.Request.PathBase + context.Request.Path + QueryString.Create(parameters) });
            }

            var user = await userManager.GetUserAsync(result.Principal) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            var app = await applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("The application details cannot be retrieved.");

            var authorizations = await authorizationManager.FindAsync(
                subject: user.Id,
                client: await applicationManager.GetIdAsync(app),
                status: Statuses.Valid,
                type: AuthorizationTypes.Permanent,
                scopes: request.GetScopes()).ToListAsync();

            switch (await applicationManager.GetConsentTypeAsync(app))
            {
                case ConsentTypes.External when authorizations.Count is 0:
                    return TypedResults.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "The logged in user is not allowed to access this client application.",
                        }));

                case ConsentTypes.Implicit:
                case ConsentTypes.External when authorizations.Count is not 0:
                case ConsentTypes.Explicit when authorizations.Count is not 0:
                    var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

                    identity.SetClaim(Claims.Subject, user.Id)
                        .SetClaim(Claims.Email, user.Email)
                        .SetClaim(Claims.Name, user.Email)
                        .SetClaim(Claims.PreferredUsername, user.UserName);

                    identity.SetScopes(request.GetScopes());
                    identity.SetResources(await scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());

                    var authorization = authorizations.LastOrDefault();
                    authorization ??= await authorizationManager.CreateAsync(identity, user.Id, await applicationManager.GetIdAsync(app), AuthorizationTypes.Permanent, identity.GetScopes());

                    identity.SetAuthorizationId(await authorizationManager.GetIdAsync(authorization));
                    identity.SetDestinations(GetDestinations);

                    return TypedResults.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
                case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
                    return TypedResults.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Interactive user consent is required.",
                        }));

                default:
                    throw new NotImplementedException(); // Explicit consent is not supported.
            }
        })
            .WithSummary("Authorize User")
            .WithDescription("Handles OIDC authorize requests.")
            .WithNoCache();

        group.MapPost("token", async Task<Results<SignInHttpResult, ForbidHttpResult>> (
            HttpContext context,
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager) =>
        {
            var request = context.GetOpenIddictServerRequest() ?? throw new InvalidOperationException("Could not retrieve token request details");
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

                return TypedResults.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the authorization code/refresh token.
                var result = await context.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                var principal = result.Principal ?? throw new InvalidOperationException("Could not retrieve current claims principal.");

                // Retrieve the user profile corresponding to the authorization code/refresh token.
                var user = await userManager.FindByIdAsync(principal.FindFirstValue(Claims.Subject) ?? throw new InvalidOperationException("Could not retrieve current user ID."));
                if (user is null)
                {
                    return TypedResults.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid.",
                        }));
                }

                // Ensure the user is still allowed to sign in.
                if (!await signInManager.CanSignInAsync(user))
                {
                    return TypedResults.Forbid(
                        authenticationSchemes: [OpenIddictServerAspNetCoreDefaults.AuthenticationScheme],
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in.",
                        }));
                }

                var identity = new ClaimsIdentity(
                    principal.Claims,
                    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

                var userRoles = await userManager.GetRolesAsync(user);

                // Override the user claims present in the principal in case they
                // changed since the authorization code/refresh token was issued.
                identity.SetClaim(Claims.Subject, user.Id)
                        .SetClaim(Claims.Email, user.Email)
                        .SetClaim(Claims.Name, user.Email)
                        .SetClaim(Claims.PreferredUsername, user.UserName)
                        .SetClaim(Claims.Role, string.Join(",", userRoles))
                        .SetClaim("Approved", user.IsApproved.ToString());

                identity.SetDestinations(GetDestinations);

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return TypedResults.SignIn(new ClaimsPrincipal(identity), authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant type is not implemented.");
        })
            .WithSummary("Get Tokens")
            .WithDescription("Handles OIDC token requests.")
            .WithNoCache();

        return builder;
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name or Claims.PreferredUsername or "Approved":
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Profile))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Email))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject.HasScope(Scopes.Roles))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
