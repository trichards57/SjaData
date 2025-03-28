// <copyright file="AccountEndpoints.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Data;
using System.Security.Claims;

namespace SjaInNumbers.Server.Endpoints;

/// <summary>
/// Contains the endpoints for the account API.
/// </summary>
public static partial class AccountEndpoints
{
    /// <summary>
    /// Maps the endpoints for the account API.
    /// </summary>
    /// <param name="builder">The builder for making the API.</param>
    /// <returns><paramref name="builder"/> to allow method chaining.</returns>
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("account")
            .AllowAnonymous();

        group.MapGet("clear", (HttpContext context, ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("AccountEndpoints");

            context.Response.Headers.TryAdd("Clear-Site-Data", "\"cache\", \"cookies\", \"storage\", \"executionContexts\", \"*\"");

            logger.LogClientStateResetRequested();

            return TypedResults.Redirect("/");
        })
            .WithSummary("Clear Site Data")
            .WithDescription("Clears the site data for a user.")
            .WithNoCache();

        group.MapGet("externalLogin", async Task<Results<RedirectHttpResult, ForbidHttpResult>> (
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            [FromQuery] string returnUrl,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("AccountEndpoints");
            var info = await signInManager.GetExternalLoginInfoAsync() ?? throw new InvalidOperationException("Error loading external login information.");
            var userId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown";

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (result.Succeeded)
            {
                LogUserLoggedIn(logger, userId, returnUrl);

                return TypedResults.Redirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                LogUserLockedOut(logger, userId);

                return TypedResults.Forbid();
            }

            var user = new ApplicationUser();
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrWhiteSpace(email))
            {
                LogErrorLoggingIn(logger, userId, "Email was empty or null.");

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
                    LogUserRegistered(logger, user.Id, email);
                    LogUserLoggedIn(logger, user.Id, returnUrl);
                    return TypedResults.Redirect(returnUrl);
                }
            }

            LogErrorLoggingIn(logger, userId, "Error with external information");
            throw new InvalidOperationException("Error loading external login information.");
        })
            .WithSummary("Conduct External Login")
            .WithDescription("Handles requests to sign in with an external login provider.")
            .WithNoCache();

        group.MapGet("login", (
            HttpContext context,
            SignInManager<ApplicationUser> signInManager,
            [FromQuery] string returnUrl,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("AccountEndpoints");

            var redirectUrl = UriHelper.BuildRelative(context.Request.PathBase, "/api/account/externalLogin", QueryString.Create("ReturnUrl", returnUrl));

            var properties = signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);

            LogUserLoginRequested(logger, "Microsoft", redirectUrl);

            return TypedResults.Challenge(properties, ["Microsoft"]);
        })
            .WithSummary("Conduct User Login")
            .WithDescription("Handles requests to sign in.  Will redirect the user to the external provider.")
            .WithNoCache();

        group.MapPost("logout", async (
            SignInManager<ApplicationUser> signInManager,
            [FromQuery] string returnUrl,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("AccountEndpoints");

            await signInManager.SignOutAsync();

            LogUserLoggedOut(logger, returnUrl);

            return TypedResults.Redirect(returnUrl);
        })
            .WithSummary("Log Out")
            .WithDescription("Handles signing the current user out.")
            .WithNoCache();

        return builder;
    }

    [LoggerMessage(1002, LogLevel.Information, "Client state reset requested.")]
    private static partial void LogClientStateResetRequested(this ILogger logger);

    [LoggerMessage(3001, LogLevel.Error, "An error was reported when user {userId} tried logging in : {error}.")]
    private static partial void LogErrorLoggingIn(this ILogger logger, string userId, string error);

    [LoggerMessage(2001, LogLevel.Warning, "User {userId} locked out.")]
    private static partial void LogUserLockedOut(this ILogger logger, string userId);

    [LoggerMessage(1004, LogLevel.Information, "User {userId} logged in and returned to {returnUrl}.")]
    private static partial void LogUserLoggedIn(this ILogger logger, string userId, string returnUrl);

    [LoggerMessage(1003, LogLevel.Information, "User logged out and returned to {returnUrl}.")]
    private static partial void LogUserLoggedOut(this ILogger logger, string returnUrl);

    [LoggerMessage(1001, LogLevel.Information, "User login requested for {provider} with return to {returnUrl}.")]
    private static partial void LogUserLoginRequested(this ILogger logger, string provider, string returnUrl);

    [LoggerMessage(1005, LogLevel.Information, "User {userId} registered using email {email}.")]
    private static partial void LogUserRegistered(this ILogger logger, string userId, string email);
}
