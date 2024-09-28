// <copyright file="NonSjaEmailOnlyAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace SJAData.Client.Validation;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class NonSjaEmailOnlyAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null)
        {
            return true;
        }

        var email = value.ToString();

        if (email?.EndsWith("@sja.org.uk") == true)
        {
            return false;
        }

        return true;
    }
}
