// <copyright file="DistrictsEndpoints.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Server.Endpoints;

public static partial class DistrictsEndpoints
{
    public static IEndpointRouteBuilder MapDistrictEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder
           .MapGroup("districts")
           .WithTags("Districts")
           .AllowAnonymous();

        group.MapGet("{id}", (IDistrictService districtService, [FromRoute]int id) =>
            districtService.GetDistrict(id))
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get District")
            .WithDescription("Gets the district with the specified ID.")
            .RequireAuthorization("Approved");

        group.MapGet(string.Empty, (IDistrictService districtService) =>
            districtService.GetAll())
            .WithSummary("Get All District")
            .WithDescription("Gets all the districts in the system.")
            .RequireAuthorization("Approved");

        group.MapPost("merge", (IDistrictService districtService, MergeDistrict mergeDistrict) =>
            districtService.MergeDistrictsAsync(mergeDistrict))
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Merge Districts")
            .WithDescription("Merges the source district into the destination district.")
            .RequireAuthorization("Admin"); // TODO : Check permissions

        group.MapPost("{id}/code", (IDistrictService districtService, [FromRoute] int id, [FromBody] string code) =>
            districtService.SetDistrictCodeAsync(id, code))
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Code")
            .WithDescription("Updates the code for a district.")
            .RequireAuthorization("Admin"); // TODO : Check permissions

        group.MapPost("{id}/name", (IDistrictService districtService, [FromRoute] int id, [FromBody] string name) =>
            districtService.SetDistrictNameAsync(id, name))
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Name")
            .WithDescription("Updates the name for a district.")
            .RequireAuthorization("Admin"); // TODO : Check permissions

        return builder;
    }
}
