using Users.Domain.Models;

namespace Users.Application.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(UserModel user);
    string GenerateRefreshToken();
}