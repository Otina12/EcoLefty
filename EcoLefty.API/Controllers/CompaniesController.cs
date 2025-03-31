using EcoLefty.Application.Authentication.Tokens.DTOs;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Application.Companies.Validators;
using EcoLefty.Application.Contracts;
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var companies = await _serviceManager.CompanyService.GetAllAsync(cancellationToken);
        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var company = await _serviceManager.CompanyService.GetByIdAsync(id, cancellationToken);
        return Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequestDto createDto, CancellationToken cancellationToken)
    {
        var validator = new CreateCompanyRequestDtoValidator();
        await validator.ValidateAndThrowAsync(createDto, HttpContext.RequestAborted);

        TokenResponseDto tokenPair = await _serviceManager.CompanyService.CreateAsync(createDto, cancellationToken);
        return Ok(new { tokens = tokenPair });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCompanyRequestDto updateDto, CancellationToken cancellationToken)
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
        var result = await _serviceManager.CompanyService.ApproveCompanyAsync(id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var deleted = await _serviceManager.CompanyService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
