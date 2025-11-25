using Application.DTOs;
using Application.DTOs.Enums;
using AutoMapper;
using Domain;

namespace Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<CustomerRisk, UserRiskDto>()
            .ForMember(dest => dest.RiskStatus,
                opt =>
                    opt.MapFrom(src => Enum.Parse<RiskStatusDto>(src.RiskStatus.ToString())));
    }
}