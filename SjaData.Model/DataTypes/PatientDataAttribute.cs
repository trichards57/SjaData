// <copyright file="PatientDataAttribute.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Compliance.Classification;

namespace SjaData.Model.DataTypes;

public class PatientDataAttribute : DataClassificationAttribute
{
    public PatientDataAttribute()
        : base(DataClassifications.PatientData)
    {
    }
}
