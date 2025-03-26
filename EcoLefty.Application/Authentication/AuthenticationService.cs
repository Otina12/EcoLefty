using EcoLefty.Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcoLefty.Application.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;

    public AuthenticationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GenerateJwtToken(Account account)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, account.AccountType.ToString()),
                new Claim(ClaimTypes.NameIdentifier, account.Id),
                new Claim(ClaimTypes.Email, account.Email!)
            };

        // TODO (if time left): change to JwtSettingsDto
        string secret = _configuration["JwtSettings:Secret"]!;
        string issuer = _configuration["JwtSettings:Issuer"]!;
        string audience = _configuration["JwtSettings:Audience"]!;
        double expiration = Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"]);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiration),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Task.FromResult(tokenString);
    }
}
