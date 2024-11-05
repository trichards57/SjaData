using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SjaInNumbers.Server.Controllers;
using SjaInNumbers.Server.Data;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace SjaInNumbers.Server.Tests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<SignInManager<ApplicationUser>> signInManager;
    private readonly Mock<UserManager<ApplicationUser>> userManager;
    private readonly Mock<IUserStore<ApplicationUser>> userStore = new();

    public AccountControllerTests()
    {
        userManager = new(userStore.Object, null, null, null, null, null, null, null, null);
        signInManager = new(userManager.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);
    }

    [Fact]
    public void Clear_SetsHeadersAndRedirects()
    {
        var controller = new AccountController(signInManager.Object, userManager.Object, userStore.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };

        var result = controller.Clear();

        result.Should().BeOfType<RedirectResult>().Which.Url.Should().Be("/");
        controller.Response.Headers.Should().ContainKey("Clear-Site-Data")
            .WhoseValue.Should().ContainSingle("\"cache\", \"cookies\", \"storage\", \"executionContexts\", \"*\"");
    }

    [Fact]
    public async Task Logout_SignsOutUserAndRedirects()
    {
        var controller = new AccountController(signInManager.Object, userManager.Object, userStore.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };

        var result = await controller.Logout(string.Empty);

        result.Should().BeOfType<LocalRedirectResult>().Which.Url.Should().Be("~/");
        signInManager.Verify(m => m.SignOutAsync(), Times.Once);
        controller.Response.Headers.Should().ContainKey("Clear-Site-Data")
            .WhoseValue.Should().ContainSingle("\"cache\", \"cookies\", \"storage\", \"executionContexts\", \"*\"");
    }

    [Fact]
    public void Login_ReturnsChallenge()
    {
        var provider = "Microsoft";
        var redirectUri = "/api/account/externalLogin?ReturnUrl=";
        var authProperties = new AuthenticationProperties();

        signInManager
            .Setup(s => s.ConfigureExternalAuthenticationProperties(provider, redirectUri, null))
            .Returns(authProperties)
            .Verifiable();

        var controller = new AccountController(signInManager.Object, userManager.Object, userStore.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };

        controller.Request.PathBase = "/";

        var result = controller.Login(provider, string.Empty);

        var challengeRes = result.Should().BeOfType<ChallengeResult>().Subject;

        signInManager.VerifyAll();

        challengeRes.Properties.Should().BeSameAs(authProperties);
        challengeRes.AuthenticationSchemes.Should().ContainSingle(provider);
    }
}
