using AutoMapper;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EcoLefty.Application.Accounts;

internal class AccountService : IAccountService
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;

    public AccountService(UserManager<Account> userManager, SignInManager<Account> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<string> RegisterAccountAsync(RegisterAccountRequestDto registerDto, AccountRole accountType)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

        if (existingUser is not null)
        {
            throw new AccountAlreadyExistsException(registerDto.Email);
        }

        var uppercaseEmail = registerDto.Email.ToUpper();

        // NOT using AutoMapper for account mapping
        var account = new Account
        {
            Email = registerDto.Email,
            NormalizedEmail = uppercaseEmail,
            UserName = registerDto.Email,
            NormalizedUserName = uppercaseEmail,
            PhoneNumber = registerDto.PhoneNumber,
            IsActive = true,
            AccountType = accountType
        };

        var registerResult = await _userManager.CreateAsync(account, registerDto.Password);

        if (!registerResult.Succeeded)
        {
            var errors = string.Join(", ", registerResult.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        await _userManager.AddToRoleAsync(account, AccountRole.User.ToString());
        await _signInManager.SignInAsync(account, false);
        return account.Id;
    }

    public async Task LoginAccountAsync(LoginAccountRequestDto loginDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(loginDto.Email);

        if (existingUser is null || !existingUser.IsActive)
        {
            throw new AccountNotFoundException(loginDto.Email);
        }

        var loginResult = await _signInManager.PasswordSignInAsync(existingUser, loginDto.Password, false, false);

        if (!loginResult.Succeeded)
        {
            throw new Exception("Couldn't login with given credentials");
        }
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
