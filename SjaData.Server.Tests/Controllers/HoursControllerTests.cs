// <copyright file="HoursControllerTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using SjaData.Server.Controllers;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.Hours;
using SjaData.Server.Services.Interfaces;
using System.Security.Claims;

namespace SjaData.Server.Tests.Controllers;

public class HoursControllerTests
{
    private readonly Mock<IHoursService> hoursService = new(MockBehavior.Strict);
    private readonly FakeLogger<HoursController> logger = new();
    private readonly HttpContext context = new DefaultHttpContext
    {
        User = new ClaimsPrincipal([new ClaimsIdentity([
            new(ClaimTypes.Name, "Test Person"),
            new(ClaimTypes.NameIdentifier, "12345"),
            new(ClaimTypes.Role, "Admin"),
        ])]),
    };

    [Fact]
    public async Task GetHoursCount_DelegatesToService()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
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
        hoursService.Setup(s => s.GetHoursCountEtagAsync(date, dateType, false)).ReturnsAsync(etag);
        hoursService.Setup(s => s.CountAsync(date, dateType, false)).ReturnsAsync(expected);
        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetHoursCount(null, date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public async Task GetHoursCount_ReturnsNotModified_IfPassedCurrentEtag()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
        var date = DateOnly.FromDateTime(DateTime.Today);
        var dateType = DateType.Month;

        hoursService.Setup(s => s.GetHoursCountEtagAsync(date, dateType, false)).ReturnsAsync(etag);
        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetHoursCount(etag, date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemNotModified);

        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public async Task GetHoursCount_ReturnsUpdate_IfPassedOldEtag()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
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
        hoursService.Setup(s => s.GetHoursCountEtagAsync(date, dateType, false)).ReturnsAsync(etag);
        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        hoursService.Setup(s => s.CountAsync(date, dateType, false)).ReturnsAsync(expected);

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetHoursCount($"\"{Guid.NewGuid()}\"", date, dateType);

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
