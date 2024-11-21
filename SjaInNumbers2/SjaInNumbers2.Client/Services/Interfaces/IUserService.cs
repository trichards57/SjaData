// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model.Users;

namespace SjaInNumbers2.Client.Services.Interfaces;

/// <summary>
/// Service for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Marks a user as approved.
    /// </summary>
    /// <param name="userId">The user to approve.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to
    /// <see langword="true" /> if the user existed and was approved, <see langword="false" /> otherwise.
    /// </returns>
    Task<bool> ApproveUserAsync(string userId);

    /// <summary>
    /// Deletes the user with the given ID.
    /// </summary>
    /// <param name="userId">The user to delete.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task DeleteUserAsync(string userId);

    /// <summary>
    /// Gets all of the users in the system.
    /// </summary>
    /// <returns>
    /// A list of all users in the system.
    /// </returns>
    IAsyncEnumerable<UserDetails> GetAll();

    /// <summary>
    /// Gets details about the given user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task<UserDetails?> GetUserAsync(string userId);

    /// <summary>
    /// Updates the user's role.
    /// </summary>
    /// <param name="userDetails">The new role details for the user.</param>
    /// <returns>
    /// <see langword="true" /> if the user existed and was updated, <see langword="false" /> otherwise.
    /// </returns>
    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
