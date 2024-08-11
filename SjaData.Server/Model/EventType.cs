// <copyright file="EventType.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaData.Server.Model;

/// <summary>
/// The type of event.
/// </summary>
public enum EventType
{
    /// <summary>
    /// An undefined event type.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// An SJA event.
    /// </summary>
    Event = 1,

    /// <summary>
    /// And NHS Ambulance Auxiliary support event.
    /// </summary>
    NHSAmbOps = 2,

    /// <summary>
    /// Any other event type.
    /// </summary>
    Other = 3,
}
