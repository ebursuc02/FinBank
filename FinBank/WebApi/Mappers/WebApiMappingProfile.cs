using Application.DTOs;
using AutoMapper;
using WebApi.DTOs;
using Application.UseCases.Commands;
using WebApi.DTOs.Request;
using WebApi.DTOs.Response;

namespace WebApi.Mappers;

public class WebApiMappingProfile : Profile
{
    public WebApiMappingProfile()
    {
        CreateMap<AccountDto, AccountResponseDto>();
        CreateMap<CreateAccountRequestDto, CreateAccountCommand>();
    }
}

