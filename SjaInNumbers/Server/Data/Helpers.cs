// <copyright file="Helpers.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Server.Data;

/// <summary>
/// Contains a set of helper methods for filtering data.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Gets all the active vehicles.
    /// </summary>
    /// <param name="vehicles">The vehicles to filter.</param>
    /// <returns>The filtered items.</returns>
    public static IQueryable<Vehicle> GetActive(this IQueryable<Vehicle> vehicles)
        => vehicles.GetNotDeleted().Where(v => !v.ForDisposal);

    /// <summary>
    /// Gets all the vehicle incidents in a given place.
    /// </summary>
    /// <param name="incidents">The vehicle incidents to filter.</param>
    /// <param name="region">The region to filter for.</param>
    /// <param name="district">The district to filter for.</param>
    /// <param name="hub">The hub to filter for.</param>
    /// <returns>The filtered items.</returns>
    public static IQueryable<VehicleIncident> GetForPlace(this IQueryable<VehicleIncident> incidents, Region region, string? district = null, string? hub = null)
    {
        if (region == Region.All)
        {
            return incidents;
        }

        var filteredIncidents = incidents
            .Where(v => v.Vehicle.Region == region);

        if (!string.IsNullOrWhiteSpace(district))
        {
            filteredIncidents = filteredIncidents.Where(v => v.Vehicle.District == district);

            if (!string.IsNullOrWhiteSpace(hub))
            {
                filteredIncidents = filteredIncidents.Where(v => v.Vehicle.Hub == hub);
            }
        }

        return filteredIncidents;
    }

    /// <summary>
    /// Gets all the vehicle incidents in a given place.
    /// </summary>
    /// <param name="incidents">The vehicle incidents to filter.</param>
    /// <param name="place">The place to filter for.</param>
    /// <returns>The filtered items.</returns>
    public static IQueryable<VehicleIncident> GetForPlace(this IQueryable<VehicleIncident> incidents, Place place)
    {
        var district = place.District.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.District;
        var hub = place.Hub.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.Hub;

        return GetForPlace(incidents, place.Region, district, hub);
    }

    /// <summary>
    /// Gets all the vehicles in a given place.
    /// </summary>
    /// <param name="vehicles">The vehicles to filter.</param>
    /// <param name="place">The place to filter for.</param>
    /// <returns>The filtered items.</returns>
    public static IQueryable<Vehicle> GetForPlace(this IQueryable<Vehicle> vehicles, Place place)
    {
        var district = place.District.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.District;
        var hub = place.Hub.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.Hub;

        return GetForPlace(vehicles, place.Region, district, hub);
    }

    /// <summary>
    /// Gets all the vehicles in a given place.
    /// </summary>
    /// <param name="vehicles">The vehicles to filter.</param>
    /// <param name="region">The region to filter for.</param>
    /// <param name="district">The district to filter for.</param>
    /// <param name="hub">The hub to filter for.</param>
    /// <returns>The filtered items.</returns>
    public static IQueryable<Vehicle> GetForPlace(this IQueryable<Vehicle> vehicles, Region region, string? district = null, string? hub = null)
    {
        if (region == Region.All)
        {
            return vehicles;
        }

        var filteredVehicles = vehicles
            .Where(v => v.Region == region);

        if (!string.IsNullOrWhiteSpace(district))
        {
            filteredVehicles = filteredVehicles.Where(v => v.District == district);

            if (!string.IsNullOrWhiteSpace(hub))
            {
                filteredVehicles = filteredVehicles.Where(v => v.Hub == hub);
            }
        }

        return filteredVehicles;
    }

    /// <summary>
    /// Gets all the items that have not been deleted.
    /// </summary>
    /// <typeparam name="T">The item type to filter by.</typeparam>
    /// <param name="items">The items to filter.</param>
    /// <returns>The filtered items.</returns>
    public static IQueryable<T> GetNotDeleted<T>(this IQueryable<T> items)
        where T : IDeletableItem
        => items.Where(v => v.Deleted == null);
}
