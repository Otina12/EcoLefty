using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Contracts;
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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _serviceManager.ApplicationUserService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _serviceManager.ApplicationUserService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationUserRequestDto createDto, CancellationToken cancellationToken)
    {
        var createdUser = await _serviceManager.ApplicationUserService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateApplicationUserRequestDto updateDto, CancellationToken cancellationToken)
    {
        var updatedUser = await _serviceManager.ApplicationUserService.UpdateAsync(id, updateDto, cancellationToken);
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _serviceManager.ApplicationUserService.DeleteAsync(id, cancellationToken);
        if (deleted)
            return NoContent();
        return NotFound();
    }
}
