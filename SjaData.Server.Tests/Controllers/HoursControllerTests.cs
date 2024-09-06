// <copyright file="HoursControllerTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Server.Controllers;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;
using System.Security.Claims;

namespace SjaData.Server.Tests.Api;

public class HoursControllerTests
{
    private readonly Mock<IHoursService> hoursService = new(MockBehavior.Strict);
    private readonly FakeLogger<HoursController> logger = new();
    private readonly int testId = 42;
    private readonly HttpContext context = new DefaultHttpContext
    {
        User = new ClaimsPrincipal([new ClaimsIdentity([
            new(ClaimTypes.Name, "Test Person"),
            new(ClaimTypes.NameIdentifier, "12345"),
            new(ClaimTypes.Role, "Admin"),
        ])]),
    };

    private readonly NewHoursEntry testNewHours = new()
    {
        Date = DateOnly.FromDateTime(DateTime.Today),
        Hours = TimeSpan.FromHours(1.25),
        Name = "Test Person",
        PersonId = 12345,
        Region = Region.NorthEast,
    };

    [Fact]
    public async Task AddHours_DelegatesToService()
    {
        hoursService.Setup(s => s.AddAsync(testNewHours)).Returns(Task.CompletedTask);
        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.AddHours(testNewHours);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemCreated);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteHours_DelegatesToService()
    {
        hoursService.Setup(s => s.DeleteAsync(testId)).Returns(Task.CompletedTask);
        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.DeleteHours(testId);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemDeleted);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetHoursCount_DelegatesToService()
    {
        var date = DateOnly.FromDateTime(DateTime.Today);
        var dateType = DateType.Month;

        var expected = new HoursCount
        {
            LastUpdate = DateTime.Now,
            Counts = new AreaDictionary<TimeSpan>(new Dictionary<string, TimeSpan>
            {
                { "NE", TimeSpan.FromHours(42) },
                { "NW", TimeSpan.FromHours(24) },
                { "SE", TimeSpan.FromHours(36) },
                { "SW", TimeSpan.FromHours(18) },
            }),
        };
        hoursService.Setup(s => s.CountAsync(date, dateType)).ReturnsAsync(expected);
        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetHoursCount(null, date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public async Task GetHoursCount_ReturnsNotModified_IfPassedNewDate()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var date = DateOnly.FromDateTime(DateTime.Today);
        var dateType = DateType.Month;

        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetHoursCount(lastModified, date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemNotModified);

        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public async Task GetHoursCount_ReturnsUpdate_IfPassedOldDate()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var date = DateOnly.FromDateTime(DateTime.Today);
        var dateType = DateType.Month;

        var expected = new HoursCount
        {
            LastUpdate = DateTime.Now,
            Counts = new AreaDictionary<TimeSpan>(new Dictionary<string, TimeSpan>
            {
                { "NE", TimeSpan.FromHours(42) },
                { "NW", TimeSpan.FromHours(24) },
                { "SE", TimeSpan.FromHours(36) },
                { "SW", TimeSpan.FromHours(18) },
            }),
        };
        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        hoursService.Setup(s => s.CountAsync(date, dateType)).ReturnsAsync(expected);

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetHoursCount(lastModified.AddSeconds(-2), date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public void GetHoursTarget_ReturnsHoursTarget()
    {
        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = controller.GetHoursTarget();

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(new HoursTarget { Target = 4000 });
    }
}
