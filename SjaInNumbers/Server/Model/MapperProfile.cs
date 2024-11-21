// <copyright file="MapperProfile.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using AutoMapper;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Server.Model;

/// <summary>
/// Profile for mapping between different classes.
/// </summary>
public class MapperProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapperProfile"/> class.
    /// </summary>
    public MapperProfile()
    {
        CreateMap<DeploymentsFileLine, NewDeployment>()
            .ForMember(d => d.DipsReference, o => o.MapFrom(s => s.DipsNumber ?? 0))
            .ForMember(d => d.DistrictId, o => o.MapFrom(s => s.District))
            .ForMember(d => d.FrontLineAmbulances, o => o.MapFrom(s => s.Ambulances));
    }
}
