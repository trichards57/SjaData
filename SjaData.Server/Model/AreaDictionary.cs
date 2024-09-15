// <copyright file="AreaDictionary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Server.Model.Converters;

namespace SjaData.Server.Model;

/// <summary>
/// A dictionary that uses region or trust names as keys.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class AreaDictionary<T>(IDictionary<string, T> dictionary) : Dictionary<string, T>(dictionary)
{
    /// <summary>
    /// Adds a value to the dictionary using the region name as the key.
    /// </summary>
    /// <param name="region">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(Region region, T value) => Add(RegionConverter.ToString(region), value);

    /// <summary>
    /// Adds a value to the dictionary using the trust name as the key.
    /// </summary>
    /// <param name="trust">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(Trust trust, T value) => Add(TrustConverter.ToString(trust), value);
}
