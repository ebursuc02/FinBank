using Domain;

namespace Application.Security.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}