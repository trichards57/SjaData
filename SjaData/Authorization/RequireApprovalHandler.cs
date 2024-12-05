// <copyright file="RequireApprovalHandler.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SjaData.Data;

namespace SjaData.Authorization;

/// <summary>
/// Handler for the <see cref="RequireApprovalRequirement"/>.
/// </summary>
/// <param name="userManager">Manager for acessing user data.</param>
public class RequireApprovalHandler(UserManager<ApplicationUser> userManager) : AuthorizationHandler<RequireApprovalRequirement>
{
    private readonly UserManager<ApplicationUser> userManager = userManager;

    /// <inheritdoc/>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireApprovalRequirement requirement)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user?.IsApproved == true)
        {
            context.Succeed(requirement);
        }
    }
}
