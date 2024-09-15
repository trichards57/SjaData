// <copyright file="PatientDataAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Compliance.Classification;

namespace SjaData.Server.DataTypes;

/// <summary>
/// Marks a property as containing patient data.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = true)]
public class PatientDataAttribute : DataClassificationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PatientDataAttribute"/> class.
    /// </summary>
    public PatientDataAttribute()
        : base(DataClassifications.PatientData)
    {
    }
}
