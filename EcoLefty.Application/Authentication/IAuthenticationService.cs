using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Entities.Identity;

namespace EcoLefty.Application.Authentication;

public interface IAuthenticationService
{
    Task<TokenResponseDto> RegisterAccountAsync(RegisterAccountRequestDto registerDto, AccountRole accountType);
    Task<TokenResponseDto> LoginAccountAsync(LoginAccountRequestDto loginDto);
    //Task ResetPassword(ResetAccountPasswordRequestDto resetPasswordDto);
    //Task ForgotPassword(string email);
    Task LogoutAsync();
    Task<string> GetAccountIdFromJwtTokenAsync(string jwtToken);
    Task AddClaimAsync(Account account, string claimType, string claimValue);
}
