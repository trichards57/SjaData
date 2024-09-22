// <copyright file="TrustConverterTests.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using SjaData.Server.Model;
using SjaData.Server.Model.Converters;

namespace SjaData.Server.Tests.Model.Converters;

public class TrustConverterTests
{
    [Fact]
    public void FromString_WithInvalidInput_ReturnsUndefined()
    {
        var actual = TrustConverter.FromString("Invalid");

        actual.Should().Be(Trust.Undefined);
    }

    [Theory]
    [InlineData("NEAS", Trust.NorthEastAmbulanceService)]
    [InlineData("NWAS", Trust.NorthWestAmbulanceService)]
    [InlineData("WMAS", Trust.WestMidlandsAmbulanceService)]
    [InlineData("EMAS", Trust.EastMidlandsAmbulanceService)]
    [InlineData("EEAST", Trust.EastOfEnglandAmbulanceService)]
    [InlineData("LAS", Trust.LondonAmbulanceService)]
    [InlineData("SECAMB", Trust.SouthEastCoastAmbulanceService)]
    [InlineData("SWAST", Trust.SouthWesternAmbulanceService)]
    [InlineData("SCAS", Trust.SouthCentralAmbulanceService)]
    [InlineData("YAS", Trust.YorkshireAmbulanceService)]
    [InlineData("WAST", Trust.WelshAmbulanceService)]
    [InlineData("SAS", Trust.ScottishAmbulanceService)]
    [InlineData("NIAS", Trust.NorthernIrelandAmbulanceService)]
    [InlineData("IWAS", Trust.IsleOfWightAmbulanceService)]
    public void FromString_WithValidInput_ReturnsRegion(string inputValue, Trust expected)
    {
        var actual = TrustConverter.FromString(inputValue);

        actual.Should().Be(expected);
    }

    [Fact]
    public void ToString_WithInvalidInput_ReturnsEmptyString()
    {
        var actual = TrustConverter.ToString((Trust)42);

        actual.Should().BeEmpty();
    }

    [Theory]
    [InlineData(Trust.NorthEastAmbulanceService, "NEAS")]
    [InlineData(Trust.NorthWestAmbulanceService, "NWAS")]
    [InlineData(Trust.WestMidlandsAmbulanceService, "WMAS")]
    [InlineData(Trust.EastMidlandsAmbulanceService, "EMAS")]
    [InlineData(Trust.EastOfEnglandAmbulanceService, "EEAST")]
    [InlineData(Trust.LondonAmbulanceService, "LAS")]
    [InlineData(Trust.SouthEastCoastAmbulanceService, "SECAMB")]
    [InlineData(Trust.SouthWesternAmbulanceService, "SWAST")]
    [InlineData(Trust.SouthCentralAmbulanceService, "SCAS")]
    [InlineData(Trust.YorkshireAmbulanceService, "YAS")]
    [InlineData(Trust.WelshAmbulanceService, "WAST")]
    [InlineData(Trust.ScottishAmbulanceService, "SAS")]
    [InlineData(Trust.NorthernIrelandAmbulanceService, "NIAS")]
    [InlineData(Trust.IsleOfWightAmbulanceService, "IWAS")]
    public void ToString_WithValidInput_ReturnsString(Trust inputValue, string expected)
    {
        var actual = TrustConverter.ToString(inputValue);

        expected.Should().Be(actual);
    }
}
