// <copyright file="HoursApiTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;
using Moq;
using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Server.Api;
using SjaData.Server.Api.Model;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;
using System.Security.Claims;

namespace SjaData.Server.Tests.Api;

public class HoursApiTests
{
    private readonly Mock<IHoursService> hoursService;
    private readonly LoggerFactory loggerFactory;
    private readonly FakeLoggerProvider loggerProvider;
    private readonly NewHoursEntry testNewHours;
    private readonly int testId;
    private HttpContextMock httpContext;

    public HoursApiTests()
    {
        hoursService = new(MockBehavior.Strict);
        httpContext = new();

        loggerProvider = new();
        loggerFactory = new LoggerFactory([loggerProvider]);
        testNewHours = new NewHoursEntry
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            Hours = TimeSpan.FromHours(1.25),
            Name = "Test Person",
            PersonId = 12345,
            Region = SjaData.Model.Region.NorthEast,
        };
        testId = 42;
    }

    [Fact]
    public async Task AddHours_DelegatesToService()
    {
        hoursService.Setup(s => s.AddAsync(testNewHours)).Returns(Task.CompletedTask);

        var result = await HoursApiExtensions.AddHours(testNewHours, hoursService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemCreated);

        result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task DeleteHours_DelegatesToService()
    {
        hoursService.Setup(s => s.DeleteAsync(testId)).Returns(Task.CompletedTask);

        var result = await HoursApiExtensions.DeleteHours(testId, hoursService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemDeleted);

        result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task GetHoursCount_DelegatesToService()
    {
        var query = default(HoursQuery);
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
        hoursService.Setup(s => s.CountAsync(query)).ReturnsAsync(expected);

        var result = await HoursApiExtensions.GetHoursCount(query, hoursService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<Ok<HoursCount>>();

        var res = (Ok<HoursCount>)result;

        res.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetHoursCount_ReturnsUpdate_IfPassedOldDate()
    {
        var lastModified = DateTimeOffset.UtcNow;

        var query = default(HoursQuery);
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
        hoursService.Setup(s => s.CountAsync(query)).ReturnsAsync(expected);

        httpContext = new HttpContextMock().SetupRequestHeaders(new HeaderDictionary
        {
            { HeaderNames.IfModifiedSince, lastModified.AddSeconds(-2).ToString("R") },
        });

        var result = await HoursApiExtensions.GetHoursCount(query, hoursService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<Ok<HoursCount>>();

        var res = (Ok<HoursCount>)result;

        res.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetHoursCount_ReturnsNotModified_IfPassedNewDate()
    {
        var lastModified = DateTimeOffset.UtcNow;

        var query = default(HoursQuery);

        hoursService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);

        httpContext = new HttpContextMock().SetupRequestHeaders(new HeaderDictionary
        {
            { HeaderNames.IfModifiedSince, lastModified.ToString("R") },
        });

        var result = await HoursApiExtensions.GetHoursCount(query, hoursService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemNotModified);

        result.Should().BeOfType<StatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }

    [Fact]
    public void GetHoursTarget_ReturnsHoursTarget()
    {
        var result = HoursApiExtensions.GetHoursTarget();

        result.Should().BeOfType<Ok<HoursTarget>>()
            .Which.Value.Should().BeEquivalentTo(new HoursTarget { Target = 4000 });
    }
}
