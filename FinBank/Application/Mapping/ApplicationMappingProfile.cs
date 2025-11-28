using Application.DTOs;
using AutoMapper;
using Domain;

namespace Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<Transfer, TransferDto>().ReverseMap();
    }
}