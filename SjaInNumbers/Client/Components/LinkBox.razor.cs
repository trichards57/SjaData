// <copyright file="LinkBox.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components;

namespace SjaInNumbers.Client.Components;

/// <summary>
/// Layout box that can be used to display a link or other content.
/// </summary>
public partial class LinkBox
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public LinkBoxColour Colour { get; set; }

    [Parameter]
    public bool? IsLink { get; set; }

    [Parameter]
    public string? Link { get; set; }

    private string StyleClasses => "link-box " + ColourToClass(Colour);

    private static string ColourToClass(LinkBoxColour colour)
    {
        return colour switch
        {
            LinkBoxColour.DarkGreen => "link-box-dark-green",
            LinkBoxColour.Yellow => "link-box-yellow",
            LinkBoxColour.Black => "link-box-black",
            LinkBoxColour.LightGray => "link-box-light-gray",
            _ => "link-box-green",
        };
    }
}
