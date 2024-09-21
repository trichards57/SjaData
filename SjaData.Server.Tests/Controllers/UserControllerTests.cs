using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using SjaData.Server.Controllers;
using SjaData.Server.Model.Users;
using SjaData.Server.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SjaData.Server.Tests.Controllers;

public class UserControllerTests
{
    private readonly HttpContext context = new DefaultHttpContext
    {
        User = new ClaimsPrincipal([new ClaimsIdentity([
            new(ClaimTypes.Name, "Test Person"),
            new(ClaimTypes.NameIdentifier, "123456"),
            new(ClaimTypes.Role, "Admin"),
        ])]),
    };

    private readonly HttpContext claimLessContext = new DefaultHttpContext
    {
        User = new ClaimsPrincipal([new ClaimsIdentity([new(ClaimTypes.NameIdentifier, "123456")])]),
    };

    private readonly Mock<IUserService> userService = new(MockBehavior.Strict);
    private readonly FakeLogger<UserController> logger = new();

    [Fact]
    public void GetAll_DelegatesToService()
    {
        var expected = new List<UserDetails> { new UserDetails { Name = "Test Person", Role = Data.Role.Admin } };

        userService.Setup(s => s.GetAll()).Returns(expected.ToAsyncEnumerable());

        var controller = new UserController(userService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = controller.GetAll();

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetMe_ReturnsUsersDetails()
    {
        var controller = new UserController(userService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = controller.GetMe();

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new CurrentUser { Name = "Test Person", Role = "Admin" });
    }

    [Fact]
    public void GetMe_WithNoClaims_ReturnsUsersDetails()
    {
        var controller = new UserController(userService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = claimLessContext } };

        var result = controller.GetMe();

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(new CurrentUser { Name = string.Empty, Role = string.Empty });
    }

    [Fact]
    public async Task UpdateUser_DelegatesToService()
    {
        var userChange = new UserRoleChange { Id = "12345", Role = Data.Role.Admin };

        userService.Setup(s => s.UpdateUserAsync(userChange)).ReturnsAsync(true);

        var controller = new UserController(userService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.UpdateUser(userChange);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateUser_WithFailedUpdate_ReturnsNotFound()
    {
        var userChange = new UserRoleChange { Id = "12345", Role = Data.Role.Admin };

        userService.Setup(s => s.UpdateUserAsync(userChange)).ReturnsAsync(false);

        var controller = new UserController(userService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.UpdateUser(userChange);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateUser_WithCurrentUserId_ReturnsBadRequest()
    {
        var userChange = new UserRoleChange { Id = "123456", Role = Data.Role.Admin };

        var controller = new UserController(userService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.UpdateUser(userChange);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
