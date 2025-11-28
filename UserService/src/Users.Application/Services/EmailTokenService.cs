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
    
    private const string Issuer = "InnoShop";
    private const string Audience = "InnoShop";
    private const string ClaimPurpose = "Purpose";
    private const string PurposeValue = "EmailConfirmation";

    public EmailTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateEmailConfirmationToken(int userId)
    {
        var keyString = _configuration["Jwt:EmailConfirmationKey"];
        if (string.IsNullOrEmpty(keyString)) throw new ArgumentNullException("Jwt:EmailConfirmationKey is missing in config");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", userId.ToString()),
            new Claim(ClaimPurpose, PurposeValue) 
        };

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int? ValidateEmailConfirmationToken(string token)
    {
        var keyString = _configuration["Jwt:EmailConfirmationKey"];
        if (string.IsNullOrEmpty(keyString)) return null;

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var key = Encoding.UTF8.GetBytes(keyString);
            
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero 
            }, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var purposeClaim = principal.FindFirst(ClaimPurpose)?.Value;
            if (purposeClaim != PurposeValue)
            {
                return null; 
            }

            var userIdClaim = principal.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return null;
            }

            return userId;
        }
        catch
        {
            return null;
        }
    }
}