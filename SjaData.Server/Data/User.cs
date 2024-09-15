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
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; }

    public string Name { get; set; }

    public Role Role { get; set; }
}

public enum Role : byte
{
    None = 0,
    Lead = 1,
    Admin = 2,
}
