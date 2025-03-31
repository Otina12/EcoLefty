using EcoLefty.Application.Contracts;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoLefty.Web.Controllers;

public class OfferController : Controller
{
    private readonly IServiceManager _serviceManager;

    public OfferController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Offer")]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var offers = await _serviceManager.OfferService.GetActiveOffersAsync(token);
        return View("Index", offers);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken token)
    {
        var offerDetailsDto = await _serviceManager.OfferService.GetByIdAsync(id, token);
        return View(offerDetailsDto);
    }

    [AuthorizeApprovedCompany]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken token)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        ViewBag.Products = await _serviceManager.ProductService.GetAllProductsOfCompanyAsync(currentUserId, token);
        return View();
    }

    [AuthorizeApprovedCompany]
    [HttpPost]
    public async Task<IActionResult> Create(CreateOfferRequestDto createOfferDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(createOfferDto);
        }

        var result = await _serviceManager.OfferService.CreateAsync(createOfferDto, token);
        return RedirectToAction("Index");
    }
}
