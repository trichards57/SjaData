// <copyright file="IdentityComponentsEndpointRouteBuilderExtensions.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SjaInNumbers.Server.Data;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Routing;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapGet("/Login", Login);
        accountGroup.MapGet("/ExternalLogin", ExternalLogin);
        accountGroup.MapPost("/Logout", Logout);

        return accountGroup;
    }

    private static async Task<IResult> ExternalLogin(
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromServices] IUserStore<ApplicationUser> userStore,
                [FromServices] UserManager<ApplicationUser> userManager,
                [FromQuery] string returnUrl)
    {
        var info = await signInManager.GetExternalLoginInfoAsync() ?? throw new InvalidOperationException("Error loading external login information.");

        // Sign in the user with this external login provider if the user already has a login.
        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            return TypedResults.Redirect(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            return TypedResults.Forbid(info.AuthenticationProperties);
        }

        var user = new ApplicationUser();
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Error loading external login information.");
        }

        await userStore.SetUserNameAsync(user, email, CancellationToken.None);
        await ((IUserEmailStore<ApplicationUser>)userStore).SetEmailAsync(user, email, CancellationToken.None);
        user.EmailConfirmed = true;

        var createResult = await userManager.CreateAsync(user);

        if (createResult.Succeeded)
        {
            createResult = await userManager.AddLoginAsync(user, info);

            if (createResult.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                return TypedResults.Redirect(returnUrl);
            }
        }

        throw new InvalidOperationException("Error loading external login information.");
    }

    private static ChallengeHttpResult Login(
                HttpContext context,
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromQuery] string returnUrl,
                [FromQuery] string provider = "Microsoft")
    {
        IEnumerable<KeyValuePair<string, StringValues>> query = [new("ReturnUrl", returnUrl)];

        var redirectUrl = UriHelper.BuildRelative(
            context.Request.PathBase,
            "/Account/ExternalLogin",
            QueryString.Create(query));

        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return TypedResults.Challenge(properties, [provider]);
    }

    private static async Task<IResult> Logout(
            SignInManager<ApplicationUser> signInManager,
            [FromForm] string returnUrl)
    {
        await signInManager.SignOutAsync();
        return TypedResults.LocalRedirect($"~/{returnUrl}");
    }
}
