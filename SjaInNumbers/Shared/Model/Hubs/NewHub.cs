// <copyright file="NewHub.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers.Shared.Model.Hubs;

public readonly record struct NewHub
{
    public int DistrictId { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; init; }
}
