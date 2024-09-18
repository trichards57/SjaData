// <copyright file="User.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SjaData.Server.Data;

/// <summary>
/// Represents a user of the system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the user's Microsoft ID.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's role.
    /// </summary>
    public Role Role { get; set; }
}
