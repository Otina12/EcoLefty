using EcoLefty.Application;
using EcoLefty.Application.Categories.DTOs;
using EcoLefty.Application.Categories.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public CategoriesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await _serviceManager.CategoryService.GetAllAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var category = await _serviceManager.CategoryService.GetByIdAsync(id, cancellationToken);
        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDto createDto, CancellationToken cancellationToken)
    {
        var validator = new CreateCategoryRequestDtoValidator();
        await validator.ValidateAndThrowAsync(createDto, HttpContext.RequestAborted);

        var createdCategory = await _serviceManager.CategoryService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryResponseDto>> Update(int id, [FromBody] UpdateCategoryRequestDto updateDto, CancellationToken cancellationToken)
    {
        var validator = new UpdateCategoryRequestDtoValidator();
        await validator.ValidateAndThrowAsync(updateDto, HttpContext.RequestAborted);

        var updatedCategory = await _serviceManager.CategoryService.UpdateAsync(id, updateDto, cancellationToken);
        return Ok(updatedCategory);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _serviceManager.CategoryService.DeleteAsync(id, cancellationToken);
        if (deleted)
            return NoContent();

        return NotFound();
    }
}
