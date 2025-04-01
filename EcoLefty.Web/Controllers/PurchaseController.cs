using EcoLefty.Application.Contracts;
using EcoLefty.Application.Purchases.DTOs;
using EcoLefty.Domain.Common.Exceptions.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoLefty.Web.Controllers;

[Authorize(Roles = "User")]
public class PurchaseController : Controller
{
    private readonly IServiceManager _serviceManager;

    public PurchaseController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken token)
    {
        var purchaseDetails = await _serviceManager.PurchaseService.GetByIdAsync(id);

        if (User.FindFirst(ClaimTypes.NameIdentifier)!.Value != purchaseDetails.Customer.Id)
            throw new BadRequestException();

        return View(purchaseDetails);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePurchaseRequestDto createPurchaseDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Offer", new { id = createPurchaseDto.OfferId });
        }

        var result = await _serviceManager.PurchaseService.CreatePurchaseAsync(createPurchaseDto, token);
        return RedirectToAction("Purchases", "User", new { id = createPurchaseDto.OfferId });
    }
}

