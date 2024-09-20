// <copyright file="UserRoleChange.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Data;

namespace SjaData.Server.Model.Users;

public readonly record struct UserRoleChange
{
    public string Id { get; init; }
    public Role Role { get; init; }
}
