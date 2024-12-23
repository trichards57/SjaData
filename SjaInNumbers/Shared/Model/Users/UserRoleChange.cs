﻿// <copyright file="UserRoleChange.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers.Shared.Model.Users;

/// <summary>
/// Represents a change to a user's role.
/// </summary>
public readonly record struct UserRoleChange
{
    /// <summary>
    /// Gets the Entra ID of the user.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Id { get; init; }

    /// <summary>
    /// Gets the new role for the user.
    /// </summary>
    [Required]
    public IList<string> Roles { get; init; }
}
