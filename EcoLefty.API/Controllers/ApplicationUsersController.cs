using EcoLefty.Application;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.ApplicationUsers.Validators;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Categories.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationUsersController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ApplicationUsersController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplicationUserResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _serviceManager.ApplicationUserService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUserDetailsResponseDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var user = await _serviceManager.ApplicationUserService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpGet("{id}/categories")]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetFollowedCategoriesByUserId(string id, CancellationToken cancellationToken)
    {
        var user = await _serviceManager.CategoryService.GetAllFollowedCategoriesByUserIdAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateApplicationUserRequestDto createDto, CancellationToken cancellationToken)
    {
        var validator = new CreateApplicationUserRequestDtoValidator();
        await validator.ValidateAndThrowAsync(createDto, HttpContext.RequestAborted);

        TokenResponseDto tokenPair = await _serviceManager.ApplicationUserService.CreateAsync(createDto, cancellationToken);
        return Ok(new { Tokens = tokenPair });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationUserResponseDto>> Update(string id, [FromBody] UpdateApplicationUserRequestDto updateDto, CancellationToken cancellationToken)
    {
        var validator = new UpdateApplicationUserRequestDtoValidator();
        await validator.ValidateAndThrowAsync(updateDto, HttpContext.RequestAborted);

        var updatedUser = await _serviceManager.ApplicationUserService.UpdateAsync(id, updateDto, cancellationToken);
        return Ok(updatedUser);
    }

    [Authorize(Roles = "Admin,User")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _serviceManager.ApplicationUserService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
