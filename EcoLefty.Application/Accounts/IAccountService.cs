using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Domain.Common.Enums;

namespace EcoLefty.Application.Accounts;

public interface IAccountService
{
    Task<string> RegisterAccountAsync(RegisterAccountRequestDto registerDto, AccountRole accountType);
    Task LoginAccountAsync(LoginAccountRequestDto loginDto);
    //Task ResetPassword(ResetAccountPasswordRequestDto resetPasswordDto);
    //Task ForgotPassword(string email);
    Task LogoutAsync();
}
