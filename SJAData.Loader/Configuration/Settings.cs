// <copyright file="Settings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Loader.Configuration;

internal class Settings
{
    public string BaseUrl { get; set; } = "https://localhost:7125";

    public Uri HoursApiPath => new($"{BaseUrl}/api/hours?api-version=1.0");
}
