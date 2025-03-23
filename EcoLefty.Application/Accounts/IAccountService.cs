using EcoLefty.Application.Accounts.DTOs;

namespace EcoLefty.Application.Accounts;

public interface IAccountService
{
    Task<string> RegisterAccountAsync(RegisterAccountRequestDto registerDto);
    Task LoginAccountAsync(LoginAccountRequestDto loginDto);
    //Task ResetPassword(ResetAccountPasswordRequestDto resetPasswordDto);
    //Task ForgotPassword(string email);
    Task LogoutAsync();
}
