// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model.Users;

namespace SjaData.Server.Services.Interfaces;

public interface IUserService
{
    IAsyncEnumerable<UserDetails> GetAll();

    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
