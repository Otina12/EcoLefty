using EcoLefty.Application;
using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Companies.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public CompaniesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyResponseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var companies = await _serviceManager.CompanyService.GetAllAsync(cancellationToken);
        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyDetailsResponseDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var company = await _serviceManager.CompanyService.GetByIdAsync(id, cancellationToken);
        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<TokenResponseDto>> Create([FromBody] CreateCompanyRequestDto createDto, CancellationToken cancellationToken)
    {
        var validator = new CreateCompanyRequestDtoValidator();
        await validator.ValidateAndThrowAsync(createDto, HttpContext.RequestAborted);

        TokenResponseDto tokenPair = await _serviceManager.CompanyService.CreateAsync(createDto, cancellationToken);
        return Ok(tokenPair);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CompanyResponseDto>> Update(string id, [FromBody] UpdateCompanyRequestDto updateDto, CancellationToken cancellationToken)
    {
        var validator = new UpdateCompanyRequestDtoValidator();
        await validator.ValidateAndThrowAsync(updateDto, HttpContext.RequestAborted);

        var updatedCompany = await _serviceManager.CompanyService.UpdateAsync(id, updateDto, cancellationToken);
        return Ok(updatedCompany);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveCompany(string id)
    {
        await _serviceManager.CompanyService.ApproveCompanyAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin,Company")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _serviceManager.CompanyService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
