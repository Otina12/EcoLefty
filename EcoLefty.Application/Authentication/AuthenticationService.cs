using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Authentication.Tokens;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Domain.Common.Enums;
using EcoLefty.Domain.Common.Exceptions;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Domain.Contracts;
using EcoLefty.Domain.Contracts.Repositories;
using EcoLefty.Domain.Entities.Auth;
using EcoLefty.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EcoLefty.Application.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthenticationService(
        UserManager<Account> userManager,
        SignInManager<Account> signInManager,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokenResponseDto> RegisterAccountAsync(RegisterAccountRequestDto dto, AccountRole accountType)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            throw new AccountAlreadyExistsException(dto.Email);

        var account = new Account
        {
            Email = dto.Email,
            NormalizedEmail = dto.Email.ToUpper(),
            UserName = dto.Email,
            NormalizedUserName = dto.Email.ToUpper(),
            PhoneNumber = dto.PhoneNumber,
            IsActive = true,
            AccountType = accountType
        };

        var result = await _userManager.CreateAsync(account, dto.Password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(account, accountType.ToString());

        var tokenPair = await _tokenService.GenerateTokenPairAsync(account);
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenPair.RefreshToken,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
            AccountId = account.Id
        };

        await _signInManager.SignInAsync(account, isPersistent: false);

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return tokenPair;
    }

    public async Task<TokenResponseDto> LoginAccountAsync(LoginAccountRequestDto loginDto)
    {
        var account = await _userManager.FindByEmailAsync(loginDto.Email);
        if (account is null || !account.IsActive)
            throw new AccountNotFoundException(loginDto.Email);

        var result = await _signInManager.CheckPasswordSignInAsync(account, loginDto.Password, false);
        if (!result.Succeeded)
            throw new Exception("Invalid login credentials");

        await _signInManager.SignInAsync(account, isPersistent: false);

        var tokenPair = await _tokenService.GenerateTokenPairAsync(account);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenPair.RefreshToken,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
            AccountId = account.Id
        };

        if (account.AccountType == AccountRole.Company)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(account.Id, false);
            if (company is null)
                throw new CompanyNotFoundException(account.Id);

            await AddClaimAsync(account, "IsApproved", company!.IsApproved.ToString());
        }

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return tokenPair;
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken)
            ?? throw new UnauthorizedException("Invalid access token.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedException("Invalid token claims.");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedException("User not found.");

        var storedToken = await _refreshTokenRepository.GetValidTokenAsync(userId, request.RefreshToken);
        if (storedToken == null)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        // Remove the old refresh token
        _refreshTokenRepository.Remove(storedToken);

        var tokenPair = await _tokenService.GenerateTokenPairAsync(user);
        var newRefresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenPair.RefreshToken,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
            AccountId = user.Id
        };

        await _refreshTokenRepository.AddAsync(newRefresh);
        await _unitOfWork.SaveChangesAsync();

        return tokenPair;
    }

    public Task<string> GetAccountIdFromJwtTokenAsync(string jwtToken)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);
        return Task.FromResult(token.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task AddClaimAsync(Account account, string type, string value)
    {
        var result = await _userManager.AddClaimAsync(account, new Claim(type, value));
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
