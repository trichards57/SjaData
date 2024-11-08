using AutoMapper;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Server.Model;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<DeploymentsFileLine, NewDeployment>()
            .ForMember(d => d.DipsReference, o => o.MapFrom(s => s.DipsNumber ?? 0))
            .ForMember(d => d.DistrictId, o => o.MapFrom(s => s.District))
            .ForMember(d => d.FrontLineAmbulances, o => o.MapFrom(s => s.Ambulances));
    }
}
