using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SjaInNumbers.Server.Data;
using System.Security.Claims;

namespace SjaInNumbers.Server.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore) : ControllerBase
{
    private readonly SignInManager<ApplicationUser> signInManager = signInManager;
    private readonly IUserStore<ApplicationUser> userStore = userStore;

    [HttpPost("login")]
    public IActionResult Login(string provider, string returnUrl)
    {
        IEnumerable<KeyValuePair<string, StringValues>> query = [new("ReturnUrl", returnUrl)];

        var redirectUrl = UriHelper.BuildRelative(HttpContext.Request.PathBase, "/Account/ExternalLogin", QueryString.Create(query));

        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, [provider]);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(string returnUrl)
    {
        await signInManager.SignOutAsync();
        return LocalRedirect($"~/{returnUrl}");
    }

    [HttpPost("externalLogin")]
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
            return LocalRedirect(returnUrl);
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
                return LocalRedirect(returnUrl);
            }
        }

        throw new InvalidOperationException("Error loading external login information.");
    }
}
