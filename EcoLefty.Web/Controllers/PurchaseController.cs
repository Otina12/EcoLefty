using EcoLefty.Application;
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken token)
    {
        var purchaseDetails = await _serviceManager.PurchaseService.GetByIdAsync(id, token);

        if (User.FindFirst(ClaimTypes.NameIdentifier)!.Value != purchaseDetails.Customer.Id)
            throw new ForbiddenException();

        return View(purchaseDetails);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePurchaseRequestDto createPurchaseDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", "Offer", new { id = createPurchaseDto.OfferId });
        }

        var curUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        await _serviceManager.PurchaseService.CreatePurchaseAsync(createPurchaseDto, token);
        return RedirectToAction("Profile", "User", new { id = curUserId });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Cancel(int id, CancellationToken token)
    {
        var curUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var purchaseDetails = await _serviceManager.PurchaseService.GetByIdAsync(id, token);

        if (curUserId != purchaseDetails.Customer.Id)
            throw new ForbiddenException();

        await _serviceManager.PurchaseService.CancelPurchaseAsync(id, token);
        return RedirectToAction("Profile", "User", new { id = curUserId });
    }
}

