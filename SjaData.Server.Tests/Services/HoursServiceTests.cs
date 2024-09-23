// <copyright file="HoursServiceTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;
using SjaData.Server.Data;
using SjaData.Server.Model;
using SjaData.Server.Services;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SjaData.Server.Tests.Services;

public sealed class HoursServiceTests : IDisposable
{
    private readonly DataContext testContext;
    private readonly FakeLogger<HoursService> logger = new();
    private readonly FakeTimeProvider timeProvider = new();

    public HoursServiceTests()
    {
        var databaseName = Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database=ambulance-numbers-{databaseName};Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        testContext = new DataContext(options);
        testContext.Database.EnsureCreated();

        timeProvider.SetUtcNow(DateTimeOffset.Parse("2023-09-05T00:00:00", CultureInfo.InvariantCulture));
    }

    public void Dispose()
    {
        testContext.Database.EnsureDeleted();
        testContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetTrendsEtagAsync_WithValidInput_ReturnsCorrectEtag(bool nhse)
    {
        await AddTestHours();

        var region = Region.SouthWest;
        var expectedLastModified = DateTimeOffset.Parse("2023-09-02T00:00:00", CultureInfo.InvariantCulture);

        var expectedStartDate = new DateOnly(2023, 8, 31); // First day of the previous month
        var expectedHashInput = $"{region}-{nhse}-{expectedStartDate}-{expectedLastModified}";
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expectedHashInput));
        var expectedEtag = $"\"{Convert.ToBase64String(expectedHash)}\"";

        var service = new HoursService(timeProvider, testContext, logger);

        var result = await service.GetTrendsEtagAsync(region, nhse);

        result.Should().Be(expectedEtag);
    }

    [Theory]
    [InlineData(Region.Undefined)]
    [InlineData((Region)42)]
    public async Task GetTrendsEtagAsync_WithInvalidRegion_ThrowsArgumentException(Region region)
    {
        var service = new HoursService(timeProvider, testContext, logger);

        var action = async () => await service.GetTrendsEtagAsync(region, true);

        await action.Should().ThrowAsync<ArgumentException>().WithParameterName("region");
    }

    [Fact]
    public async Task GetTrendsEtagAsync_OnJanuaryFirst_ReturnsCorrectEtag()
    {
        timeProvider.SetUtcNow(DateTimeOffset.Parse("2024-01-01T00:00:00", CultureInfo.InvariantCulture));

        await AddTestHours();

        var region = Region.SouthWest;
        var expectedLastModified = DateTimeOffset.Parse("2023-09-02T00:00:00", CultureInfo.InvariantCulture);
        var nhse = false;

        var expectedStartDate = new DateOnly(2023, 12, 31); // First day of the previous month
        var expectedHashInput = $"{region}-{nhse}-{expectedStartDate}-{expectedLastModified}";
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expectedHashInput));
        var expectedEtag = $"\"{Convert.ToBase64String(expectedHash)}\"";

        var service = new HoursService(timeProvider, testContext, logger);

        var result = await service.GetTrendsEtagAsync(region, nhse);

        result.Should().Be(expectedEtag);
    }

    [Fact]
    public async Task GetTrendsEtagAsync_WithNoData_ReturnsCorrectEtag()
    {
        var region = Region.SouthWest;
        var expectedLastModified = DateTimeOffset.MinValue;
        var nhse = false;

        var expectedStartDate = new DateOnly(2023, 8, 31); // First day of the previous month
        var expectedHashInput = $"{region}-{nhse}-{expectedStartDate}-{expectedLastModified}";
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expectedHashInput));
        var expectedEtag = $"\"{Convert.ToBase64String(expectedHash)}\"";

        var service = new HoursService(timeProvider, testContext, logger);

        var result = await service.GetTrendsEtagAsync(region, nhse);

        result.Should().Be(expectedEtag);
    }

    private async Task AddTestHours()
    {
        testContext.People.AddRange(
            new Person { Id = 1234, DeletedAt = null, FirstName = "First Name 1", LastName = "Last Name 1", District = "District 1", Region = Region.SouthWest, UpdatedAt = DateTimeOffset.Parse("2023-06-01T00:00:00", CultureInfo.InvariantCulture) },
            new Person { Id = 1235, DeletedAt = null, FirstName = "First Name 2", LastName = "Last Name 2", District = "District 2", Region = Region.SouthWest, UpdatedAt = DateTimeOffset.Parse("2023-06-02T00:00:00", CultureInfo.InvariantCulture) },
            new Person { Id = 1236, DeletedAt = null, FirstName = "First Name 3", LastName = "Last Name 3", District = "District 3", Region = Region.WestMidlands, UpdatedAt = DateTimeOffset.Parse("2023-06-03T00:00:00", CultureInfo.InvariantCulture) });

        testContext.Hours.AddRange(
            new HoursEntry { Date = new DateOnly(2023, 8, 31), Region = Region.SouthWest, Hours = 1, DeletedAt = null, UpdatedAt = DateTimeOffset.Parse("2023-09-01T00:00:00", CultureInfo.InvariantCulture), PersonId = 1234, Trust = Trust.Undefined },
            new HoursEntry { Date = new DateOnly(2023, 9, 1), Region = Region.SouthWest, Hours = 2, DeletedAt = null, UpdatedAt = DateTimeOffset.Parse("2023-09-02T00:00:00", CultureInfo.InvariantCulture), PersonId = 1234, Trust = Trust.Undefined },
            new HoursEntry { Date = new DateOnly(2023, 8, 30), Region = Region.SouthWest, Hours = 3, DeletedAt = null, UpdatedAt = DateTimeOffset.Parse("2023-08-31T00:00:00", CultureInfo.InvariantCulture), PersonId = 1235, Trust = Trust.Undefined });

        await testContext.SaveChangesAsync();
    }
}
