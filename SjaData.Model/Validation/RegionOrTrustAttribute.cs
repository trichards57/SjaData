// <copyright file="RegionOrTrustAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SjaData.Model.Validation;

public class RegionOrTrustAttribute : ValidationAttribute
{
    public override string FormatErrorMessage(string name)
    {
        return "Exactly one of region or trust must be set.";
    }

    public override bool IsValid(object? value)
    {
        return value is IRegionAndTrust entry &&
            ((entry.Region != Region.Undefined && entry.Trust == Trust.Undefined)
            || (entry.Region == Region.Undefined && entry.Trust != Trust.Undefined));
    }
}
