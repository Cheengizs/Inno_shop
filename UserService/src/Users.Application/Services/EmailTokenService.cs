using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Services.Interfaces;

namespace Users.Application.Services;

public class EmailTokenService : IEmailTokenService
{
    private readonly IConfiguration _configuration;

    public EmailTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateEmailConfirmationToken(int userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:EmailConfirmationKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", userId.ToString()),
            new Claim("Purpose", "EmailConfirmation")
        };

        var token = new JwtSecurityToken(
            issuer: "InnoShop",
            audience: "InnoShop",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateEmailConfirmationToken(string token, out int userId)
    {
        userId = 0;
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:EmailConfirmationKey"]);
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "InnoShop",
                ValidateAudience = true,
                ValidAudience = "InnoShop",
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true
            }, out var validatedToken);
            
            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            userId = int.Parse(principal.FindFirst("UserId")!.Value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}