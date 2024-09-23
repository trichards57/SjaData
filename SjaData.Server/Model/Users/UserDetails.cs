// <copyright file="UserDetails.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Data;

namespace SjaData.Server.Model.Users;

/// <summary>
/// Represents the details of a user of the system.
/// </summary>
public readonly record struct UserDetails
{
    /// <summary>
    /// Gets the user's ID as reported by Microsoft Entra.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Gets the user's name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the user's assigned role.
    /// </summary>
    public Role Role { get; init; }
}
