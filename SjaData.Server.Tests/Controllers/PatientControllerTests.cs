// <copyright file="PatientControllerTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using SjaData.Model;
using SjaData.Model.Patient;
using SjaData.Server.Controllers;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;
using System.Security.Claims;

namespace SjaData.Server.Tests.Api;

public class PatientControllerTests
{
    private readonly FakeLogger<PatientController> logger = new();
    private readonly Mock<IPatientService> patientService = new(MockBehavior.Strict);
    private readonly int testId = 42;
    private readonly HttpContext context = new DefaultHttpContext
    {
        User = new ClaimsPrincipal([new ClaimsIdentity([
            new(ClaimTypes.Name, "Test Person"),
            new(ClaimTypes.NameIdentifier, "12345"),
            new(ClaimTypes.Role, "Admin"),
        ])]),
    };

    private readonly NewPatient testNewPatient = new()
    {
        CallSign = "WR123",
        Date = DateOnly.FromDateTime(DateTime.Today),
        Id = 12345,
        Region = Region.NorthEast,
    };

    [Fact]
    public async Task AddHours_DelegatesToService()
    {
        patientService.Setup(s => s.AddAsync(testNewPatient)).Returns(Task.CompletedTask);
        var controller = new PatientController(patientService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.AddPatient(testNewPatient);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemCreated);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeletePatient_DelegatesToService()
    {
        patientService.Setup(s => s.DeleteAsync(testId)).Returns(Task.CompletedTask);
        var controller = new PatientController(patientService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.DeleteHours(testId);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemDeleted);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetPatientCount_DelegatesToService()
    {
        var date = DateOnly.FromDateTime(DateTime.Today);
        var dateType = DateType.Month;
        var trust = Trust.Undefined;
        var region = Region.SouthWest;
        var outcome = Outcome.Conveyed;
        var eventType = EventType.Event;

        var expected = new PatientCount
        {
            LastUpdate = DateTime.Now,
            Counts = new AreaDictionary<int>(new Dictionary<string, int>
            {
                { "NE", 42 },
                { "NW", 24 },
                { "SE", 36 },
                { "SW", 18 },
            }),
        };

        patientService.Setup(s => s.CountAsync(region, trust, eventType, outcome, date, dateType)).ReturnsAsync(expected);
        var controller = new PatientController(patientService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetPatientCount(null, region, trust, eventType, outcome, date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }

    [Fact]
    public async Task GetPatientCount_ReturnsNotModified_IfPassedNewDate()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var date = DateOnly.FromDateTime(DateTime.Today);
        var dateType = DateType.Month;
        var trust = Trust.Undefined;
        var region = Region.SouthWest;
        var outcome = Outcome.Conveyed;
        var eventType = EventType.Event;

        patientService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        var controller = new PatientController(patientService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetPatientCount(lastModified, region, trust, eventType, outcome, date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemNotModified);

        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public async Task GetPatientCount_ReturnsUpdate_IfPassedOldDate()
    {
        var lastModified = DateTimeOffset.UtcNow;
        var date = DateOnly.FromDateTime(DateTime.Today);
        var dateType = DateType.Month;
        var trust = Trust.Undefined;
        var region = Region.SouthWest;
        var outcome = Outcome.Conveyed;
        var eventType = EventType.Event;

        var expected = new PatientCount
        {
            LastUpdate = DateTime.Now,
            Counts = new AreaDictionary<int>(new Dictionary<string, int>
            {
                { "NE", 42 },
                { "NW", 24 },
                { "SE", 36 },
                { "SW", 18 },
            }),
        };
        patientService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        patientService.Setup(s => s.CountAsync(region, trust, eventType, outcome, date, dateType)).ReturnsAsync(expected);
        var controller = new PatientController(patientService.Object, logger) { ControllerContext = new ControllerContext { HttpContext = context } };

        var result = await controller.GetPatientCount(lastModified.AddSeconds(-2), region, trust, eventType, outcome, date, dateType);

        logger.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expected);
    }
}
