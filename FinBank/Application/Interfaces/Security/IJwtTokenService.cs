using Application.DTOs;

namespace Application.Interfaces.Security;

public interface IJwtTokenService
{
    string GenerateToken(UserDto user);
}