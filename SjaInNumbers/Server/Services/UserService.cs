// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Users;

namespace SjaInNumbers.Server.Services;

/// <summary>
/// Service for managing user information.
/// </summary>
/// <param name="contextFactory">The factory to create a data context containing the information.</param>
/// <param name="userManager">A manager to handle user operations.</param>
public class UserService(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> contextFactory = contextFactory;
    private readonly UserManager<ApplicationUser> userManager = userManager;

    /// <inheritdoc/>
    public async Task<bool> ApproveUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        user.IsApproved = true;

        await userManager.UpdateAsync(user);

        return true;
    }

    /// <inheritdoc/>
    public async Task DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return;
        }

        await userManager.DeleteAsync(user);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<UserDetails> GetAll()
    {
        using var context = await contextFactory.CreateDbContextAsync();
        var items = await context.Users.ToListAsync();

        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item.Email))
            {
                continue;
            }

            yield return new UserDetails
            {
                Email = item.Email,
                Id = item.Id,
                Roles = await userManager.GetRolesAsync(item),
                IsApproved = item.IsApproved,
            };
        }
    }

    /// <inheritdoc/>
    public async Task<UserDetails?> GetUserAsync(string userId)
    {
        using var context = await contextFactory.CreateDbContextAsync();

        var user = await context.Users.FindAsync(userId);

        if (user == null || user.Email == null)
        {
            return null;
        }

        return new UserDetails
        {
            Email = user.Email,
            Id = user.Id,
            Roles = await userManager.GetRolesAsync(user),
            IsApproved = user.IsApproved,
        };
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateUserAsync(UserRoleChange userDetails)
    {
        var user = await userManager.FindByIdAsync(userDetails.Id);

        if (user == null)
        {
            return false;
        }

        var actualRoles = await userManager.GetRolesAsync(user);

        if (string.IsNullOrEmpty(userDetails.Role) && actualRoles.Count > 0)
        {
            await userManager.RemoveFromRolesAsync(user, actualRoles);
            return true;
        }

        if (actualRoles.Count == 1 && actualRoles.Contains(userDetails.Role))
        {
            return true;
        }

        await userManager.RemoveFromRolesAsync(user, actualRoles);
        await userManager.AddToRoleAsync(user, userDetails.Role);

        return true;
    }
}
