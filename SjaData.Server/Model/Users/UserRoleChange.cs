// <copyright file="UserRoleChange.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Data;
using System.ComponentModel.DataAnnotations;

namespace SjaData.Server.Model.Users;

public readonly record struct UserRoleChange
{
    [Required(AllowEmptyStrings = false)]
    public string Id { get; init; }

    [Required(AllowEmptyStrings = false)]
    [EnumDataType(typeof(Role))]
    public Role Role { get; init; }
}
