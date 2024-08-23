// <copyright file="PatientApiTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Net.Http.Headers;
using Moq;
using SjaData.Model.Patient;
using SjaData.Server.Api;
using SjaData.Server.Api.Model;
using SjaData.Server.Logging;
using SjaData.Server.Services.Exceptions;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Tests.Api;

public class PatientApiTests
{
    private readonly Mock<IPatientService> patientService;
    private readonly LoggerFactory loggerFactory;
    private readonly FakeLoggerProvider loggerProvider;
    private readonly NewPatient testNewPatient;
    private readonly int testId;
    private HttpContextMock httpContext;

    public PatientApiTests()
    {
        patientService = new(MockBehavior.Strict);
        httpContext = new();

        loggerProvider = new();
        loggerFactory = new LoggerFactory([loggerProvider]);
        testNewPatient = new NewPatient
        {
            CallSign = "WR123",
            Date = DateOnly.FromDateTime(DateTime.Today),
            Id = 12345,
            Region = SjaData.Model.Region.NorthEast,
        };
        testId = 42;
    }

    [Fact]
    public async Task AddHours_DelegatesToService()
    {
        patientService.Setup(s => s.AddAsync(testNewPatient)).Returns(Task.CompletedTask);

        var result = await PatientApiExtensions.AcceptPatient(testNewPatient, patientService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemCreated);

        result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task AddHours_HandlesDuplicateId()
    {
        patientService.Setup(s => s.AddAsync(testNewPatient)).Throws(new DuplicateIdException());

        var result = await PatientApiExtensions.AcceptPatient(testNewPatient, patientService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.DuplicateIdProvided);

        result.Should().BeOfType<Conflict<ProblemDetails>>();
    }

    [Fact]
    public async Task DeletePatient_DelegatesToService()
    {
        patientService.Setup(s => s.DeleteAsync(testId)).Returns(Task.CompletedTask);

        var result = await PatientApiExtensions.DeletePatient(testId, patientService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemDeleted);

        result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task GetPatientCount_DelegatesToService()
    {
        var query = default(PatientQuery);
        var expected = new PatientCount
        {
            LastUpdate = DateTime.Now,
            Counts = new Dictionary<string, int>
            {
                { "NorthEast", 42 },
                { "NorthWest", 24 },
                { "SouthEast", 36 },
                { "SouthWest", 18 },
            }.AsReadOnly(),
        };
        patientService.Setup(s => s.CountAsync(query)).ReturnsAsync(expected);

        var result = await PatientApiExtensions.GetPatientCount(query, patientService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<Ok<PatientCount>>();

        var res = (Ok<PatientCount>)result;

        res.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetPatientCount_ReturnsUpdate_IfPassedOldDate()
    {
        var lastModified = DateTimeOffset.UtcNow;

        var query = default(PatientQuery);
        var expected = new PatientCount
        {
            LastUpdate = DateTime.Now,
            Counts = new Dictionary<string, int>
            {
                { "NorthEast", 42 },
                { "NorthWest", 24 },
                { "SouthEast", 36 },
                { "SouthWest", 18 },
            }.AsReadOnly(),
        };
        patientService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);
        patientService.Setup(s => s.CountAsync(query)).ReturnsAsync(expected);

        httpContext = new HttpContextMock().SetupRequestHeaders(new HeaderDictionary
        {
            { HeaderNames.IfModifiedSince, lastModified.AddSeconds(-2).ToString("R") },
        });

        var result = await PatientApiExtensions.GetPatientCount(query, patientService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemFound);

        result.Should().BeOfType<Ok<PatientCount>>();

        var res = (Ok<PatientCount>)result;

        res.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetPatientCount_ReturnsNotModified_IfPassedNewDate()
    {
        var lastModified = DateTimeOffset.UtcNow;

        var query = default(PatientQuery);

        patientService.Setup(s => s.GetLastModifiedAsync()).ReturnsAsync(lastModified);

        httpContext = new HttpContextMock().SetupRequestHeaders(new HeaderDictionary
        {
            { HeaderNames.IfModifiedSince, lastModified.ToString("R") },
        });

        var result = await PatientApiExtensions.GetPatientCount(query, patientService.Object, httpContext, loggerFactory);

        loggerProvider.Collector.GetSnapshot().Should().ContainSingle(l => l.Id.Id == EventCodes.ItemNotModified);

        result.Should().BeOfType<StatusCodeHttpResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status304NotModified);
    }
}
