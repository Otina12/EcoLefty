using AutoMapper;
using EcoLefty.Application.Contracts;
using EcoLefty.Application.Offers.DTOs;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoLefty.Web.Controllers;

public class OfferController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IMapper _mapper;

    public OfferController(IServiceManager serviceManager, IMapper mapper)
    {
        _serviceManager = serviceManager;
        _mapper = mapper;
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

    [AuthorizeApprovedCompany]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken token)
    {
        var offerDetails = await _serviceManager.OfferService.GetByIdAsync(id, token);

        if (User.FindFirst(ClaimTypes.NameIdentifier)!.Value != offerDetails.Company.Id)
            throw new BadRequestException();

        var updateOfferDto = _mapper.Map<UpdateOfferRequestDto>(offerDetails);
        return View(updateOfferDto);
    }

    [AuthorizeApprovedCompany]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateOfferRequestDto updateOfferDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(updateOfferDto);
        }

        var result = await _serviceManager.OfferService.UpdateAsync(id, updateOfferDto, token);
        return RedirectToAction("Index");
    }
}
