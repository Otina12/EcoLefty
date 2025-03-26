using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AuthenticationController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginAccountRequestDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var token = await _serviceManager.AccountService.LoginAccountAsync(loginDto);
        return Ok(new { Token = token });
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _serviceManager.AccountService.LogoutAsync();
        return Ok(new { Message = "Logged out successfully." });
    }

    [Authorize]
    [HttpGet("test-cur-user-id")]
    public async Task<string> AuthTest(string token)
    {
        return await _serviceManager.AccountService.GetUserIdFromJwtTokenAsync(token);
    }
}
