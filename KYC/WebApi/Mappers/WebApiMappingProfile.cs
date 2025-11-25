using Application.DTOs;
using Application.DTOs.Enums;
using AutoMapper;
using Domain;
using WebApi.DTOs;

namespace WebApi.Mappers;

public class WebApiMappingProfile : Profile
{
    public WebApiMappingProfile()
    {
        CreateMap<UserRiskDto, UserRiskResponseDto>()
            .ForMember(dest => dest.RiskStatuses,
                opt =>
                    opt.MapFrom(src => src.RiskStatus.ToString()));
    }
}