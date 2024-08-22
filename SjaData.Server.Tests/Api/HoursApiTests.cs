// <copyright file="HoursApiTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using HttpContextMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using SjaData.Model.Hours;
using SjaData.Server.Api;
using SjaData.Server.Logging;
using SjaData.Server.Services.Interfaces;
using System.Security.Claims;

namespace SjaData.Server.Tests.Api;

public class HoursApiTests
{
    private readonly Mock<IHoursService> hoursService;
    private readonly HttpContextMock httpContext;
    private readonly LoggerFactory loggerFactory;
    private readonly FakeLoggerProvider loggerProvider;
    private readonly NewHoursEntry testNewHours;
    private readonly int testId;
    private readonly string testUserId;

    public HoursApiTests()
    {
        hoursService = new();
        httpContext = new();
        testUserId = "test-user";
        httpContext.UserMock.IdentityMock.AddClaim(new Claim(ClaimTypes.NameIdentifier, testUserId));

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
        var result = await HoursApiExtensions.AddHours(testNewHours, hoursService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().Contain(l => l.Id.Id == EventCodes.ItemCreated);

        result.Should().BeOfType<NoContent>();
        hoursService.Verify(s => s.AddAsync(testNewHours), Times.Once);
    }

    [Fact]
    public async Task DeleteHours_DelegatesToService()
    {
        var result = await HoursApiExtensions.DeleteHours(testId, hoursService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().Contain(l => l.Id.Id == EventCodes.ItemDeleted);

        result.Should().BeOfType<NoContent>();
        hoursService.Verify(s => s.DeleteAsync(testId), Times.Once);
    }
}
