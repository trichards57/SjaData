// -----------------------------------------------------------------------
// <copyright file="OpenIdWorkerSettings.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace SjaData.Server.Services;

/// <summary>
/// The settings for the Open ID worker.
/// </summary>
public class OpenIdWorkerSettings
{
    /// <summary>
    /// Gets or sets the VOR Uploader's client ID.
    /// </summary>
    public required string VorUploaderClientId { get; set; }

    /// <summary>
    /// Gets or sets the VOR Uploader's secret.
    /// </summary>
    public required string VorUploaderClientSecret { get; set; }
}
