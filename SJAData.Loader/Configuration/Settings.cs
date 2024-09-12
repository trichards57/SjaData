// <copyright file="Settings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Spectre.Console.Cli;
using System.ComponentModel;

namespace SJAData.Loader.Configuration;

internal class Settings : CommandSettings
{
    private const string DefaultBaseUrl = "https://localhost:7125";

    [Description($"URI to upload to.  Defaults to {DefaultBaseUrl}")]
    [CommandOption("-u|--uri")]
    public string BaseUrl { get; init; } = DefaultBaseUrl;

    [Description("Input file containing hours data.")]
    [CommandArgument(0, "[inputFile]")]
    public string InputFile { get; init; } = string.Empty;

    [Description("Input file containing person data.")]
    [CommandArgument(1, "[personFile]")]
    public string PersonFile { get; init; } = string.Empty;

    public Uri HoursApiPath => new($"{BaseUrl}/api/hours?api-version=1.0");
}
