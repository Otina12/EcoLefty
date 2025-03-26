using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Authentication;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EcoLefty.Application.Accounts
{
    internal class AccountService : IAccountService
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IAuthenticationService _authenticationService;

        public AccountService(
            UserManager<Account> userManager,
            SignInManager<Account> signInManager,
            IAuthenticationService authenticationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationService = authenticationService;
        }

        public async Task<string> RegisterAccountAsync(RegisterAccountRequestDto registerDto, AccountRole accountType)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                throw new AccountAlreadyExistsException(registerDto.Email);
            }

            var uppercaseEmail = registerDto.Email.ToUpper();

            // manual mapping (not using AutoMapper)
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

            await _userManager.AddToRoleAsync(account, accountType.ToString());

            var token = await _authenticationService.GenerateJwtToken(account);
            return token;
        }

        public async Task<string> LoginAccountAsync(LoginAccountRequestDto loginDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(loginDto.Email);
            if (existingUser is null || !existingUser.IsActive)
            {
                throw new AccountNotFoundException(loginDto.Email);
            }

            var loginResult = await _signInManager.CheckPasswordSignInAsync(existingUser, loginDto.Password, false);
            if (!loginResult.Succeeded)
            {
                throw new Exception("Invalid login credentials");
            }

            var token = await _authenticationService.GenerateJwtToken(existingUser);
            return token;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public Task<string> GetUserIdFromJwtTokenAsync(string jwtToken)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
            if (token == null)
                throw new UnauthorizedException("Invalid token");

            return Task.FromResult(token.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}
