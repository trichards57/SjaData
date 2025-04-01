// <copyright file="AreaSelector.razor.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components;
using SjaInNumbers.Client.Converters;

namespace SjaInNumbers.Client.Components;

/// <summary>
/// Component that allows the user to select one or more NHSE areas or regions.
/// </summary>
public partial class AreaSelector
{
    private readonly HashSet<string> selectedAreas = [];
    private bool expandNhse = false;
    private bool expandRegions = false;

    [Parameter]
    public required HashSet<string> ActualAreas { get; set; }

    [Parameter]
    public EventCallback<HashSet<string>> SelectedAreasChanged { get; set; }

    private IEnumerable<string> ActualNhseAreas => ActualAreas.Where(LabelConverters.IsTrust);

    private IEnumerable<string> ActualRegions => ActualAreas.Where(LabelConverters.IsRegion);

    private void ClearAreas()
    {
        selectedAreas.Clear();
        SelectedAreasChanged.InvokeAsync(selectedAreas);
    }

    private void SelectAllNhse()
    {
        selectedAreas.Clear();
        selectedAreas.UnionWith(ActualNhseAreas);
        SelectedAreasChanged.InvokeAsync(selectedAreas);
    }

    private void SelectAllRegions()
    {
        selectedAreas.Clear();
        selectedAreas.UnionWith(ActualRegions);
        SelectedAreasChanged.InvokeAsync(selectedAreas);
    }

    private void ToggleArea(string area)
    {
        if (!selectedAreas.Remove(area))
        {
            selectedAreas.Add(area);
        }

        SelectedAreasChanged.InvokeAsync(selectedAreas);
    }

    private void ToggleExpandNhse() => expandNhse = !expandNhse;

    private void ToggleExpandRegion() => expandRegions = !expandRegions;
}
