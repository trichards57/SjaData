// <copyright file="Role.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Data;

/// <summary>
/// Represents a user's role.
/// </summary>
public enum Role : byte
{
    /// <summary>
    /// No role - a standard user.
    /// </summary>
    None = 0,

    /// <summary>
    /// A lead user. Can access additional information.
    /// </summary>
    Lead = 1,

    /// <summary>
    /// An administrator.  Can update and manage access.
    /// </summary>
    Admin = 2,
}
