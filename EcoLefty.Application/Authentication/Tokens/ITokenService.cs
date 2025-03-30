using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Domain.Entities.Identity;
using System.Security.Claims;

namespace EcoLefty.Application.Authentication.Tokens;

public interface ITokenService
{
    Task<TokenResponseDto> GenerateTokenPairAsync(Account account);
    Task<string> GenerateAccessToken(Account account);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}