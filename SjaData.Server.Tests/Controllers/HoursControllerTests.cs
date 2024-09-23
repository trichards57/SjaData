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
using System.Text;

namespace SjaData.Server.Tests.Controllers;

public class HoursControllerTests
{
    private const string TestCsv = "Location,Shift Date,Shift,Post,Name,IDNUMBER,Crew Type,Callsign,Required,Relief,Shift Length,Crew Name,Remarks\r\n EDB - Long Eaton Carnival ,08/06/2024,1030-2230,Event Cover BL EAC 1,,EXT025,Event Cover Amb,,Y,N,12:00,,\r\n EDB - Long Eaton Carnival ,08/06/2024,1030-2230,Event Cover Crew 1,,EXT026,Event Cover Amb,,Y,N,12:00,,CMS Medical\r\n";

    private const string TestCsvBadContent = "Location,Shift Date,Shift,Post,Name,IDNUMBER,Crew Type,Callsign,Required,Relief,Shift Length,Crew Name,Remarks\r\n EDB - Long Eaton Carnival ,08/06/2024,1030-2230,Event Cover BL EAC 1,,EXT025,Event Cover Amb,,Y,N,12:00,,\r\n EDB - Long Eaton Carnival ,08/06/2024,1030-2230,Event Cover Crew 1,,EXT026,Event Cover Amb,,Y,N,ABS,,CMS Medical\r\n";

    private const string TestCsvBadHeader = "Locatin,Shift Date,Shift,Post,Name,IDNUMBER,Crew Type,Callsign,Required,Relief,Shift Length,Crew Name,Remarks\r\n EDB - Long Eaton Carnival ,08/06/2024,1030-2230,Event Cover BL EAC 1,,EXT025,Event Cover Amb,,Y,N,12:00,,\r\n EDB - Long Eaton Carnival ,08/06/2024,1030-2230,Event Cover Crew 1,,EXT026,Event Cover Amb,,Y,N,12:00,,CMS Medical\r\n";

    private readonly HttpContext context = new DefaultHttpContext
    {
        User = new ClaimsPrincipal([new ClaimsIdentity([
            new(ClaimTypes.Name, "Test Person"),
            new(ClaimTypes.NameIdentifier, "12345"),
            new(ClaimTypes.Role, "Admin"),
        ])]),
    };

    private readonly Mock<IHoursService> hoursService = new(MockBehavior.Strict);
    private readonly FakeLogger<HoursController> logger = new();

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

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);
    }

    [Fact]
    public async Task GetTrends_WithInvalidRegion_ReturnsBadRequest()
    {
        var controller = new HoursController(hoursService.Object, logger);

        var result = await controller.GetTrends(null, (Region)42);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetTrends_WithUndefinedRegion_ReturnsBadRequest()
    {
        var controller = new HoursController(hoursService.Object, logger);

        var result = await controller.GetTrends(null, Region.Undefined);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetTrends_DelegatesToService()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
        var region = Region.SouthWest;

        var expected = new Trends() { Hours = new Dictionary<string, double[]> { { "national", [1, 2, 3, 4, 5] } } };

        hoursService.Setup(s => s.GetTrendsEtagAsync(region, false)).ReturnsAsync(etag);
        hoursService.Setup(s => s.GetTrendsAsync(region, false)).ReturnsAsync(expected);
        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetTrends(null, region);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public async Task GetTrends_ReturnsNotModified_IfPassedCurrentEtag()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
        var region = Region.SouthWest;

        hoursService.Setup(s => s.GetTrendsEtagAsync(region, false)).ReturnsAsync(etag);
        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetTrends(etag, region);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemNotModified);

        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public async Task GetTrends_ReturnsUpdate_IfPassedOldEtag()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var etag = $"\"{Guid.NewGuid()}\"";
        var region = Region.SouthWest;

        var expected = new Trends() { Hours = new Dictionary<string, double[]> { { "national", [1, 2, 3, 4, 5] } } };

        hoursService.Setup(s => s.GetTrendsEtagAsync(region, false)).ReturnsAsync(etag);
        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        hoursService.Setup(s => s.GetTrendsAsync(region, false)).ReturnsAsync(expected);

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetTrends($"\"{Guid.NewGuid()}\"", region);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public async Task ReceiveHoursFile_WithBadContent_ReturnsCount()
    {
        hoursService.Setup(s => s.AddHours(It.Is<IAsyncEnumerable<HoursFileLine>>(l => l.CountAsync(CancellationToken.None).Result == 2))).ReturnsAsync(2);

        var testFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(TestCsvBadContent)), 0, TestCsv.Length, "file", "hours.csv");

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.ReceiveHoursFile(testFile);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.FileUploaded);
        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.FileUploadFailed);
    }

    [Fact]
    public async Task ReceiveHoursFile_WithBadHeader_ReturnsCount()
    {
        hoursService.Setup(s => s.AddHours(It.Is<IAsyncEnumerable<HoursFileLine>>(l => l.CountAsync(CancellationToken.None).Result == 2))).ReturnsAsync(2);

        var testFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(TestCsvBadHeader)), 0, TestCsv.Length, "file", "hours.csv");

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.ReceiveHoursFile(testFile);

        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.FileUploaded);
        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.FileUploadFailed);
    }

    [Fact]
    public async Task ReceiveHoursFile_WithValidFile_ReturnsCount()
    {
        hoursService.Setup(s => s.AddHours(It.Is<IAsyncEnumerable<HoursFileLine>>(l => l.CountAsync(CancellationToken.None).Result == 2))).ReturnsAsync(2);

        var testFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(TestCsv)), 0, TestCsv.Length, "file", "hours.csv");

        var controller = new HoursController(hoursService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.ReceiveHoursFile(testFile);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new CountResponse { Count = 2 });

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.FileUploaded);
        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.FileUploadSuccess);
    }
}
