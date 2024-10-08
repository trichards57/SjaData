﻿// <copyright file="UserDetails.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Client.Model.Users;

/// <summary>
/// Represents the details of a user of the system.
/// </summary>
public readonly record struct UserDetails
{
    /// <summary>
    /// Gets the user's ID.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Gets the user's assigned role.
    /// </summary>
    public IList<string> Roles { get; init; }

    public bool IsApproved { get; init; }
}