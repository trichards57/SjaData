// <copyright file="MinimumAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace SjaData.Model.Validation;

public class GreaterThanAttribute(int minimum) : ValidationAttribute
{
    public int Minimum { get; } = minimum;

    public override bool IsValid(object? value)
    {
        return (value is int i && i > Minimum)
            || (value is long l && l > Minimum)
            || (value is float f && f > Minimum)
            || (value is double d && d > Minimum)
            || (value is decimal dec && dec > Minimum)
            || (value is BigInteger bi && bi > Minimum)
            || (value is TimeSpan t && t.TotalSeconds > Minimum);
    }
}
