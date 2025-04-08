using AutoMapper;
using EcoLefty.Application;
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
    public async Task<IActionResult> GetAll(OfferSearchDto searchModel, CancellationToken token)
    {
        await PrepareOfferSearchDto(searchModel, token);
        var offers = await _serviceManager.OfferService.GetAllAsync(searchModel, token);

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
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Create(CreateOfferRequestDto createOfferDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ViewBag.Products = await _serviceManager.ProductService.GetAllProductsOfCompanyAsync(currentUserId, token); // reinitialize products dropdown
            return View(createOfferDto);
        }

        await _serviceManager.OfferService.CreateAsync(createOfferDto, token);
        return RedirectToAction("Index");
    }

    [AuthorizeApprovedCompany]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken token)
    {
        var offerDetails = await _serviceManager.OfferService.GetByIdAsync(id, token);

        if (User.FindFirst(ClaimTypes.NameIdentifier)!.Value != offerDetails.Company.Id)
            throw new ForbiddenException();

        var updateOfferDto = _mapper.Map<UpdateOfferRequestDto>(offerDetails);
        return View(updateOfferDto);
    }

    [AuthorizeApprovedCompany]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateOfferRequestDto updateOfferDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            ViewBag.Products = await _serviceManager.ProductService.GetAllProductsOfCompanyAsync(currentUserId, token); // reinitialize products dropdown
            return View(updateOfferDto);
        }

        await _serviceManager.OfferService.UpdateAsync(id, updateOfferDto, token);
        return RedirectToAction("Index");
    }

    [AuthorizeApprovedCompany]
    public async Task<IActionResult> Cancel(int id, CancellationToken token)
    {
        await _serviceManager.OfferService.CancelAsync(id, token);
        return RedirectToAction("Index");
    }

    [AuthorizeApprovedCompany]
    public async Task<IActionResult> Delete(int id, CancellationToken token)
    {
        await _serviceManager.OfferService.DeleteAsync(id, token);
        return RedirectToAction("Index");
    }

    private async Task PrepareOfferSearchDto(OfferSearchDto searchModel, CancellationToken token)
    {
        ViewBag.Companies = await _serviceManager.CompanyService.GetAllAsync(token);
        ViewBag.Categories = await _serviceManager.CategoryService.GetAllAsync(token);
        ViewBag.CurrentSearch = searchModel.SearchText;
        ViewBag.CurrentCompanyId = searchModel.CompanyId;
        ViewBag.CurrentCategoryId = searchModel.CategoryId;
        ViewBag.CurrentSort = searchModel.SortByColumn;
        ViewBag.CurrentSortDirection = searchModel.SortByAscending;
        ViewBag.OnlyActive = searchModel.OnlyActive;
        ViewBag.OnlyFollowedCategories = searchModel.OnlyFollowedCategories;
    }
}
