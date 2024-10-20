﻿// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model.Users;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IUserService
{
    Task<bool> ApproveUserAsync(string userId);

    Task DeleteUserAsync(string userId);

    IAsyncEnumerable<UserDetails> GetAll();

    Task<UserDetails?> GetUserAsync(string userId);

    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
