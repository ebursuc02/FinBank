using Application.DTOs;
using Domain;

namespace Application.Security.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(UserDto user);
}