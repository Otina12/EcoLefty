using EcoLefty.Application;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Application.Products.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ProductsController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _serviceManager.ProductService.GetAllAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDetailsResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _serviceManager.ProductService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    [Authorize(Roles = "Company")]
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductRequestDto createDto, CancellationToken cancellationToken)
    {
        var validator = new CreateProductRequestDtoValidator();
        await validator.ValidateAndThrowAsync(createDto, HttpContext.RequestAborted);

        var createdProduct = await _serviceManager.ProductService.CreateAsync(createDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    [Authorize(Roles = "Company")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductResponseDto>> Update(int id, [FromBody] UpdateProductRequestDto updateDto, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductRequestDtoValidator();
        await validator.ValidateAndThrowAsync(updateDto, HttpContext.RequestAborted);

        var updatedProduct = await _serviceManager.ProductService.UpdateAsync(id, updateDto, cancellationToken);
        return Ok(updatedProduct);
    }

    [Authorize(Roles = "Admin,Company")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _serviceManager.ProductService.DeleteAsync(id, cancellationToken);
        if (deleted)
            return NoContent();

        return NotFound();
    }
}
