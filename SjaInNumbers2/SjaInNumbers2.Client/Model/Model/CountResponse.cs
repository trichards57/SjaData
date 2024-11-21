// <copyright file="CountResponse.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json.Serialization;

namespace SjaInNumbers2.Model;

/// <summary>
/// Represents a response to an API request that returns a count.
/// </summary>
public readonly record struct CountResponse
{
    /// <summary>
    /// Gets the count of items.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; init; }
}
