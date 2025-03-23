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
    private readonly IMapper _mapper;

    public AccountService(UserManager<Account> userManager, SignInManager<Account> signInManager, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<string> RegisterAccountAsync(RegisterAccountRequestDto registerDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);

        if (existingUser is not null)
        {
            throw new AccountAlreadyExistsException(registerDto.Email);
        }

        var employee = _mapper.Map<RegisterAccountRequestDto, Account>(registerDto);
        var registerResult = await _userManager.CreateAsync(employee, registerDto.Password);

        if (!registerResult.Succeeded)
        {
            var errors = string.Join(", ", registerResult.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }

        await _userManager.AddToRoleAsync(employee, AccountRole.User.ToString());
        await _signInManager.SignInAsync(employee, false);
        return employee.Id;
    }

    public async Task LoginAccountAsync(LoginAccountRequestDto loginDto)
    {
        var existingUser = await _userManager.FindByEmailAsync(loginDto.Email);

        if (existingUser is null || !existingUser.IsActive)
        {
            throw new AccountNotFoundException();
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
