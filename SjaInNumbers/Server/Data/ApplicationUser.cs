// <copyright file="ApplicationUser.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Identity;

namespace SjaInNumbers.Server.Data;

public class ApplicationUser : IdentityUser
{
    public bool IsApproved { get; set; }
}
