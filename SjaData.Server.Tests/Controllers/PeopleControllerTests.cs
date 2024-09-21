using FluentAssertions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using SjaData.Server.Controllers;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.Hours;
using SjaData.Server.Model.People;
using SjaData.Server.Services;
using SjaData.Server.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SjaData.Server.Tests.Controllers;

public class PeopleControllerTests
{
    private readonly HttpContext context = new DefaultHttpContext
    {
        User = new ClaimsPrincipal([new ClaimsIdentity([
            new(ClaimTypes.Name, "Test Person"),
            new(ClaimTypes.NameIdentifier, "12345"),
            new(ClaimTypes.Role, "Admin"),
        ])]),
    };

    private readonly Mock<IPersonService> personService = new(MockBehavior.Strict);
    private readonly FakeLogger<PeopleController> logger = new();

    [Fact]
    public async Task GetReports_DelegatesToTheService()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
        var region = Region.SouthWest;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        IList<PersonReport> expected = [
            new PersonReport() { Name = "Test 2", Hours = [1, 2, 3, 4], HoursThisYear = 5, MonthsSinceLastActive = 2 },
            new PersonReport() { Name = "Test 2", Hours = [5, 6, 7, 8], HoursThisYear = 9, MonthsSinceLastActive = 1 },
        ];

        personService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        personService.Setup(s => s.GetPeopleReportsEtagAsync(date, region)).ReturnsAsync(etag);
        personService.Setup(s => s.GetPeopleReportsAsync(date, region)).Returns(expected.ToAsyncEnumerable());
        var controller = new PeopleController(personService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetReports(null, region);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetReports_WhenPassedCurrentEtag_ReturnsNotModified()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
        var date = DateOnly.FromDateTime(DateTime.Today);
        var region = Region.SouthWest;

        personService.Setup(s => s.GetPeopleReportsEtagAsync(date, region)).ReturnsAsync(etag);
        personService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);

        var controller = new PeopleController(personService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetReports(etag, region);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemNotModified);

        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public async Task GetReports_WhenPassedOldEtag_ReturnsUpdate()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
        var date = DateOnly.FromDateTime(DateTime.Today);
        var region = Region.SouthWest;

        IList<PersonReport> expected = [
            new PersonReport() { Name = "Test 2", Hours = [1, 2, 3, 4], HoursThisYear = 5, MonthsSinceLastActive = 2 },
            new PersonReport() { Name = "Test 2", Hours = [5, 6, 7, 8], HoursThisYear = 9, MonthsSinceLastActive = 1 },
        ];
        personService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        personService.Setup(s => s.GetPeopleReportsEtagAsync(date, region)).ReturnsAsync(etag);
        personService.Setup(s => s.GetPeopleReportsAsync(date, region)).Returns(expected.ToAsyncEnumerable());

        var controller = new PeopleController(personService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetReports($"\"{Guid.NewGuid()}\"", region);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetReports_WithInvalidRegion_ReturnsBadRequest()
    {
        var controller = new PeopleController(personService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetReports(null, (Region)42);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetReports_WithUndefinedRegion_ReturnsBadRequest()
    {
        var controller = new PeopleController(personService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetReports(null, Region.Undefined);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
