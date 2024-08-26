// <copyright file="StarRedactor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.Compliance.Redaction;

namespace SjaData.Server.Logging;

/// <summary>
/// Represents a redactor that replaces the input with a fixed number of stars.
/// </summary>
public class StarRedactor : Redactor
{
    private const string Stars = "****";

    /// <inheritdoc/>
    public override int GetRedactedLength(ReadOnlySpan<char> input) => Stars.Length;

    /// <inheritdoc/>
    public override int Redact(ReadOnlySpan<char> source, Span<char> destination)
    {
        Stars.CopyTo(destination);

        return Stars.Length;
    }
}
