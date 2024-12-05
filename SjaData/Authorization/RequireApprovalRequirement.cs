// <copyright file="RequireApprovalRequirement.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authorization;

namespace SjaData.Authorization;

/// <summary>
/// Represents a requirement that a user must have their account approved before they can access a resource.
/// </summary>
public class RequireApprovalRequirement : IAuthorizationRequirement
{
}
