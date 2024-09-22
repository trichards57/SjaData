// <copyright file="RegionConverterTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using SjaData.Server.Model;
using SjaData.Server.Model.Converters;

namespace SjaData.Server.Tests.Model.Converters;

public class RegionConverterTests
{
    [Fact]
    public void FromString_WithInvalidInput_ReturnsUndefined()
    {
        var actual = RegionConverter.FromString("Invalid");

        actual.Should().Be(Region.Undefined);
    }

    [Theory]
    [InlineData("NE", Region.NorthEast)]
    [InlineData("NW", Region.NorthWest)]
    [InlineData("WM", Region.WestMidlands)]
    [InlineData("EM", Region.EastMidlands)]
    [InlineData("EOE", Region.EastOfEngland)]
    [InlineData("LON", Region.London)]
    [InlineData("SE", Region.SouthEast)]
    [InlineData("SW", Region.SouthWest)]
    public void FromString_WithValidInput_ReturnsRegion(string inputValue, Region expected)
    {
        var actual = RegionConverter.FromString(inputValue);

        actual.Should().Be(expected);
    }

    [Fact]
    public void ToString_WithInvalidInput_ReturnsEmptyString()
    {
        var actual = RegionConverter.ToString((Region)42);

        actual.Should().BeEmpty();
    }

    [Theory]
    [InlineData(Region.NorthEast, "NE")]
    [InlineData(Region.NorthWest, "NW")]
    [InlineData(Region.WestMidlands, "WM")]
    [InlineData(Region.EastMidlands, "EM")]
    [InlineData(Region.EastOfEngland, "EOE")]
    [InlineData(Region.London, "LON")]
    [InlineData(Region.SouthEast, "SE")]
    [InlineData(Region.SouthWest, "SW")]
    public void ToString_WithValidInput_ReturnsString(Region inputValue, string expected)
    {
        var actual = RegionConverter.ToString(inputValue);

        expected.Should().Be(actual);
    }
}
