using EcoLefty.Application;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Application.Purchases.Validators;
using EcoLefty.Domain.Common.Exceptions.Base;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoLefty.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : Controller
{
    private readonly IServiceManager _serviceManager;

    public PurchasesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseDetailsResponseDto>> GetById(int id, CancellationToken token)
    {
        var purchaseDetails = await _serviceManager.PurchaseService.GetByIdAsync(id, token);

        if (User.FindFirst(ClaimTypes.NameIdentifier)!.Value != purchaseDetails.Customer.Id)
            throw new ForbiddenException();

        return Ok(purchaseDetails);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePurchaseRequestDto createPurchaseDto, CancellationToken token)
    {
        var validator = new CreatePurchaseRequestDtoValidator();
        await validator.ValidateAndThrowAsync(createPurchaseDto, HttpContext.RequestAborted);

        var purchaseDetailsDto = await _serviceManager.PurchaseService.CreatePurchaseAsync(createPurchaseDto, token);
        return CreatedAtAction(nameof(GetById), new { id = purchaseDetailsDto.Id }, purchaseDetailsDto);
    }

    [Authorize]
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken token)
    {
        await _serviceManager.PurchaseService.CancelPurchaseAsync(id, token);
        return Ok(new { cancelled = true });
    }
}
