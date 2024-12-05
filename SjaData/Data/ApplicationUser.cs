// <copyright file="ApplicationUser.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Identity;

namespace SjaInNumbers.Server.Data;

/// <summary>
/// Represents a user in the application.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Gets or sets a value indicating whether the user has been approved.
    /// </summary>
    public bool IsApproved { get; set; }
}
