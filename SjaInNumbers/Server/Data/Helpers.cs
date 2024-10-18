// <copyright file="Helpers.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Server.Data;

public static class Helpers
{
    public static IQueryable<Vehicle> GetActive(this IQueryable<Vehicle> vehicles)
        => vehicles.GetNotDeleted().Where(v => !v.ForDisposal);

    public static IQueryable<T> GetNotDeleted<T>(this IQueryable<T> items)
        where T : IDeletableItem
        => items.Where(v => v.Deleted == null);

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

    public static IQueryable<VehicleIncident> GetForPlace(this IQueryable<VehicleIncident> incidents, Place place)
    {
        var district = place.District.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.District;
        var hub = place.Hub.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.Hub;

        return GetForPlace(incidents, place.Region, district, hub);
    }

    public static IQueryable<Vehicle> GetForPlace(this IQueryable<Vehicle> vehicles, Place place)
    {
        var district = place.District.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.District;
        var hub = place.Hub.Equals("all", StringComparison.OrdinalIgnoreCase) ? null : place.Hub;

        return GetForPlace(vehicles, place.Region, district, hub);
    }

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
}
