using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Accounts.Validators;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginAccountRequestDto loginDto)
    {
        var validator = new LoginAccountRequestDtoValidator();
        await validator.ValidateAndThrowAsync(loginDto, HttpContext.RequestAborted);

        TokenResponseDto tokenPair = await _authenticationService.LoginAccountAsync(loginDto);
        return Ok(new { Tokens = tokenPair });
    }

    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _authenticationService.LogoutAsync();
        return Ok(new { Message = "Logged out successfully." });
    }

    [Authorize]
    [HttpGet("test-cur-user-id")]
    public async Task<string> AuthTest(string token)
    {
        return await _authenticationService.GetAccountIdFromJwtTokenAsync(token);
    }
}
