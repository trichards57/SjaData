// <copyright file="CountResponse.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json.Serialization;

namespace SjaData.Server.Model;

public readonly record struct CountResponse
{
    [JsonPropertyName("count")]
    public int Count { get; init; }
}
