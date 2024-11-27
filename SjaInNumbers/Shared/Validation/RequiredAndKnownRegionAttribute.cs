// <copyright file="RequiredAndKnownRegionAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;
using System.ComponentModel.DataAnnotations;

namespace SjaInNumbers.Shared.Validation;

/// <summary>
/// Validation attribute to ensure that a region is required and not <see cref="Region.Undefined"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class RequiredAndKnownRegionAttribute : ValidationAttribute
{
    /// <inheritdoc/>
    public override bool IsValid(object? value)
    {
        return value is Region region && Enum.IsDefined(region) && region != Region.Undefined;
    }
}
