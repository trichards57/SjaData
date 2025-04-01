// <copyright file="LinkBoxes.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components;

namespace SjaInNumbers.Client.Components;

/// <summary>
/// A group of link boxes.
/// </summary>
public partial class LinkBoxes
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public LinkBoxSize Size { get; set; }

    private string StyleClasses => "link-boxes link-boxes-" + Size.ToString().ToLower();
}
