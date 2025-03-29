// <copyright file="DeploymentsEndpoints.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Deployments;
using System.Globalization;

namespace SjaInNumbers.Server.Endpoints;

/// <summary>
/// Contains the endpoints for the deployments API.
/// </summary>
public static partial class DeploymentsEndpoints
{
    /// <summary>
    /// Maps the endpoints for the deployments API.
    /// </summary>
    /// <param name="builder">The builder for making the API.</param>
    /// <returns><paramref name="builder"/> to allow method chaining.</returns>
    public static IEndpointRouteBuilder MapDeploymentEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("deployments")
            .WithTags("Deployments")
            .AllowAnonymous();

        group.MapGet("national", (IDeploymentService deploymentService) =>
             deploymentService.GetNationalSummaryAsync())
            .WithSummary("National Deployments Summary")
            .WithDescription("Gets the national deployments summary.")
            .RequireAuthorization("Approved");

        group.MapGet("peaks", (IDeploymentService deploymentService) =>
            deploymentService.GetPeakLoadsAsync())
            .WithSummary("Peak Event Loads")
            .WithDescription("Gets the peak event loads for the last year.")
            .RequireAuthorization("Approved");

        group.MapPost(string.Empty, async Task<Results<Ok<CountResponse>, ProblemHttpResult>> (
            IDeploymentService deploymentService,
            IDistrictService districtService,
            IFormFile file,
            IMapper mapper) =>
        {
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
            csv.Context.RegisterClassMap<DeploymentsFileLineMap>();

            try
            {
                return TypedResults.Ok(await deploymentService.AddDeployments(
                    csv.GetRecordsAsync<DeploymentsFileLine>()
                        .Select(d => mapper.Map<NewDeployment>(d))));
            }
            catch (CsvHelperException)
            {
                return TypedResults.Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
            }
        })
            .WithSummary("Upload Deployments")
            .WithDescription("Receives a CSV file containing deployment data for processing.")
            .RequireAuthorization("Admin");

        return builder;
    }
}
