// <copyright file="CurrentUser.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model.Users;

/// <summary>
/// Represents the current user and their role.
/// </summary>
public readonly record struct CurrentUser
{
    /// <summary>
    /// Gets the current user's name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the current user's role.
    /// </summary>
    public string Role { get; init; }
}
