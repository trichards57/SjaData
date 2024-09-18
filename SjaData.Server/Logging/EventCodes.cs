// <copyright file="EventCodes.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Logging;

/// <summary>
/// Contains the logging event codes for the application.
/// </summary>
public static class EventCodes
{
    /// <summary>
    /// An item was created.
    /// </summary>
    public const int ItemCreated = 1001;

    /// <summary>
    /// An item was modified.
    /// </summary>
    public const int ItemModified = 1002;

    /// <summary>
    /// An item was deleted.
    /// </summary>
    public const int ItemDeleted = 1003;

    /// <summary>
    /// An item had not been modified and so not returned.
    /// </summary>
    public const int ItemNotModified = 1004;

    /// <summary>
    /// An item was found.
    /// </summary>
    public const int ItemFound = 1005;

    /// <summary>
    /// A duplicate item was found.
    /// </summary>
    public const int DuplicateIdProvided = 2001;

    /// <summary>
    /// The item was not found.
    /// </summary>
    public const int ItemNotFound = 2002;

    /// <summary>
    /// An update file has been received.
    /// </summary>
    public const int FileUploaded = 3001;

    /// <summary>
    /// An update file was accepted and successfully loaded.
    /// </summary>
    public const int FileUploadSuccess = 3002;

    /// <summary>
    /// An update file could not be loaded.
    /// </summary>
    public const int FileUploadFailed = 3003;
}
