// <copyright file="IUserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model.Users;

namespace SjaInNumbers.Server.Services.Interfaces;

/// <summary>
/// Represents a service for managing users.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Approves a user's access to the system.
    /// </summary>
    /// <param name="userId">The user to approve.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to true
    /// if successful, or false if the user could not be found.
    /// </returns>
    Task<bool> ApproveUserAsync(string userId);

    /// <summary>
    /// Deletes a user from the system.
    /// </summary>
    /// <param name="userId">The user to delete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteUserAsync(string userId);

    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <returns>The users in the system.</returns>
    IAsyncEnumerable<UserDetails> GetAll();

    /// <summary>
    /// Gets details on a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the
    /// user's details, or null if they could not be found.
    /// </returns>
    Task<UserDetails?> GetUserAsync(string userId);

    /// <summary>
    /// Updates a user in the system.
    /// </summary>
    /// <param name="userDetails">The update.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// Resolves to true if successful, or false if the user could not be found.
    /// </returns>
    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
