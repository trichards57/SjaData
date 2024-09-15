// <copyright file="DataClassifications.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Compliance.Classification;

namespace SjaData.Server.DataTypes;

/// <summary>
/// Classifications for data to allow redacting.
/// </summary>
public static class DataClassifications
{
    /// <summary>
    /// Gets a value indicating that the data is patient data.
    /// </summary>
    public static DataClassification PatientData { get; } = new DataClassification("PatientDataTaxonomy", "PatientData");
}
