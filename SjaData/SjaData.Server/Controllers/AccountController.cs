// <copyright file="AccountController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SjaData.Server.Controllers.Filters;
using SjaData.Server.Data;
using System.Security.Claims;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller that manages user account actions.
/// </summary>
/// <param name="signInManager">Manager for authentication actions.</param>
/// <param name="userManager">Manager for user account actions.</param>
/// <param name="userStore">Store for user data.</param>
/// <param name="logger">The logger to write to.</param>
[Route("api/account")]
[ApiController]
[AllowAnonymous]
public sealed partial class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, ILogger<AccountController> logger) : ControllerBase
{
    private readonly ILogger logger = logger;
    private readonly SignInManager<ApplicationUser> signInManager = signInManager;
    private readonly IUserStore<ApplicationUser> userStore = userStore;

    /// <summary>
    /// Clears the site data for a user.
    /// </summary>
    /// <returns>The result of the action.</returns>
    [HttpGet("clear")]
    [NotCachedFilter]
    public IActionResult Clear()
    {
        Response.Headers.TryAdd("Clear-Site-Data", "\"cache\", \"cookies\", \"storage\", \"executionContexts\", \"*\"");

        LogClientStateResetRequested();

        return Redirect("/");
    }

    /// <summary>
    /// Handles requests to sign in with an external login provider.
    /// </summary>
    /// <param name="returnUrl">The URL to return to.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("externalLogin")]
    [NotCachedFilter]
    public async Task<IActionResult> ExternalLogin(string returnUrl)
    {
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
            LogUserLoggedIn(userId, returnUrl);

            return Redirect(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            LogUserLockedOut(userId);

            return Forbid(userId);
        }

        var user = new ApplicationUser();
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            LogErrorLoggingIn(userId, "Email was empty or null.");

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
                LogUserRegistered(user.Id, email);
                LogUserLoggedIn(user.Id, returnUrl);
                return Redirect(returnUrl);
            }
        }

        LogErrorLoggingIn(userId, "Error with external information");
        throw new InvalidOperationException("Error loading external login information.");
    }

    /// <summary>
    /// Handles requests to sign in.  Redirects the user to the external authentication.
    /// </summary>
    /// <param name="provider">The log in provider.</param>
    /// <param name="returnUrl">The URL to return to afterwards.</param>
    /// <returns>The result of the action.</returns>
    [HttpGet("login")]
    [NotCachedFilter]
    public IActionResult Login(string provider, string returnUrl)
    {
        IEnumerable<KeyValuePair<string, StringValues>> query = [new("ReturnUrl", returnUrl)];

        var redirectUrl = UriHelper.BuildRelative(HttpContext.Request.PathBase, "/api/account/externalLogin", QueryString.Create(query));

        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        LogUserLoginRequested(provider, redirectUrl);

        return Challenge(properties, provider);
    }

    /// <summary>
    /// Handles requests to sign out.
    /// </summary>
    /// <param name="returnUrl">The URL to return to afterwards.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpPost("logout")]
    [NotCachedFilter]
    public async Task<IActionResult> Logout(string returnUrl)
    {
        await signInManager.SignOutAsync();

        LogUserLoggedOut(returnUrl);

        return LocalRedirect($"~/{returnUrl}");
    }

    [LoggerMessage(1002, LogLevel.Information, "Client state reset requested.")]
    private partial void LogClientStateResetRequested();

    [LoggerMessage(3001, LogLevel.Error, "An error was reported when user {userId} tried logging in : {error}.")]
    private partial void LogErrorLoggingIn(string userId, string error);

    [LoggerMessage(2001, LogLevel.Warning, "User {userId} locked out.")]
    private partial void LogUserLockedOut(string userId);

    [LoggerMessage(1004, LogLevel.Information, "User {userId} logged in.")]
    private partial void LogUserLoggedIn(string userId, string returnUrl);

    [LoggerMessage(1003, LogLevel.Information, "User logged out.")]
    private partial void LogUserLoggedOut(string returnUrl);

    [LoggerMessage(1001, LogLevel.Information, "User login requested for {provider}.")]
    private partial void LogUserLoginRequested(string provider, string returnUrl);

    [LoggerMessage(1005, LogLevel.Information, "User {userId} registered.")]
    private partial void LogUserRegistered(string userId, string email);
}
