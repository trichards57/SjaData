// <copyright file="AccountController.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SjaInNumbers.Server.Data;
using System.Security.Claims;

namespace SjaInNumbers.Server.Controllers;

/// <summary>
/// Controller that manages user account actions.
/// </summary>
/// <param name="signInManager">Manager for authentication actions.</param>
/// <param name="userManager">Manager for user account actions.</param>
/// <param name="userStore">Store for user data.</param>
[Route("api/account")]
[ApiController]
public sealed partial class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore) : ControllerBase
{
    private readonly SignInManager<ApplicationUser> signInManager = signInManager;
    private readonly IUserStore<ApplicationUser> userStore = userStore;

    /// <summary>
    /// Handles requests to sign in.  Redirects the user to the external authentication.
    /// </summary>
    /// <param name="provider">The log in provider.</param>
    /// <param name="returnUrl">The URL to return to afterwards.</param>
    /// <returns>The result of the action.</returns>
    [HttpGet("login")]
    public IActionResult Login(string provider, string returnUrl)
    {
        IEnumerable<KeyValuePair<string, StringValues>> query = [new("ReturnUrl", returnUrl)];

        var redirectUrl = UriHelper.BuildRelative(HttpContext.Request.PathBase, "/api/account/externalLogin", QueryString.Create(query));

        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
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
    public async Task<IActionResult> Logout(string returnUrl)
    {
        await signInManager.SignOutAsync();
        return LocalRedirect($"~/{returnUrl}");
    }

    /// <summary>
    /// Handles requests to sign in with an external login provider.
    /// </summary>
    /// <param name="returnUrl">The URL to return to.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. Resolves to the result of the action.
    /// </returns>
    [HttpGet("externalLogin")]
    public async Task<IActionResult> ExternalLogin(string returnUrl)
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
            return Redirect(returnUrl);
        }
        else if (result.IsLockedOut)
        {
            return Forbid();
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
                return Redirect(returnUrl);
            }
        }

        throw new InvalidOperationException("Error loading external login information.");
    }
}
