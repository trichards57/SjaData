// <copyright file="ExceptionHelpers.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Runtime.CompilerServices;

namespace SjaInNumbers2.Helpers;

/// <summary>
/// Helpers to simplify operations with exceptions.
/// </summary>
public static class ExceptionHelpers
{
    /// <summary>
    /// Throws an <see cref="ArgumentOutOfRangeException"/> if the value is undefined in the enum.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to check.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <exception cref="ArgumentOutOfRangeException">If the value is undefined or zero.</exception>
    public static void ThrowIfUndefined<TEnum>(TEnum value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value for {paramName} ({value}) is not defined in the enum {typeof(TEnum).Name}.");
        }

        if (value.Equals(Enum.Parse<TEnum>("Undefined")))
        {
            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must not be Undefined.");
        }
    }
}
