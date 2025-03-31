using EcoLefty.Application.Contracts;
using EcoLefty.Application.Purchases.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

[Authorize(Roles = "User")]
public class PurchasesController : Controller
{
    private readonly IServiceManager _serviceManager;

    public PurchasesController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
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

