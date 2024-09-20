// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Model.Users;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

public class UserService(DataContext context) : IUserService
{
    private readonly DataContext context = context;

    /// <inheritdoc/>
    public IAsyncEnumerable<UserDetails> GetAll()
    {
        return context.Users.Select(u => new UserDetails { Id = u.Id, Name = u.Name, Role = u.Role }).AsAsyncEnumerable();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateUserAsync(UserRoleChange userDetails)
    {
        var user = await context.Users.FindAsync(userDetails.Id);

        if (user == null)
        {
            return false;
        }

        user.Role = userDetails.Role;
        await context.SaveChangesAsync();
        return true;
    }
}
