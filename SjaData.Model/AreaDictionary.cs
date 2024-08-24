// <copyright file="AreaDictionary.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaData.Model.Converters;

namespace SjaData.Model;

public class AreaDictionary<T> : Dictionary<string, T>
{
    public AreaDictionary(IDictionary<string, T> dictionary)
        : base(dictionary)
    {
    }

    public void Add(Region region, T value) => Add(RegionConverter.ToString(region), value);

    public void Add(Trust trust, T value) => Add(TrustConverter.ToString(trust), value);
}
