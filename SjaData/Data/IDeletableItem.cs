// <copyright file="IDeletableItem.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Data;

/// <summary>
/// Represents an item that can be soft-deleted.
/// </summary>
public interface IDeletableItem
{
    /// <summary>
    /// Gets or sets the date the item was deleted, or <see langword="null"/> if it has not been deleted.
    /// </summary>
    DateTimeOffset? Deleted { get; set; }
}
