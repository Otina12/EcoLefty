using AutoMapper;
using EcoLefty.Application.Contracts;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Domain.Common.Exceptions.Base;
using EcoLefty.Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoLefty.Web.Controllers;
public class ProductController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IMapper _mapper;

    public ProductController(IServiceManager serviceManager, IMapper mapper)
    {
        _serviceManager = serviceManager;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Product")]
    public async Task<IActionResult> GetAll(CancellationToken token)
    {
        var offers = await _serviceManager.ProductService.GetAllAsync(token);
        return View("Index", offers);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken token)
    {
        var productDetailsDto = await _serviceManager.ProductService.GetByIdAsync(id, token);
        return View(productDetailsDto);
    }

    [AuthorizeApprovedCompany]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categoryList = await _serviceManager.CategoryService.GetAllAsync();
        ViewBag.Categories = categoryList;
        return View();
    }

    [AuthorizeApprovedCompany]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequestDto createProductDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(createProductDto);
        }

        var result = await _serviceManager.ProductService.CreateAsync(createProductDto, token);
        return RedirectToAction("Index");
    }

    [AuthorizeApprovedCompany]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken token)
    {
        var productDetails = await _serviceManager.ProductService.GetByIdAsync(id, token);

        if (User.FindFirst(ClaimTypes.NameIdentifier)!.Value != productDetails.Company.Id)
            throw new BadRequestException();

        var updateProductDto = _mapper.Map<UpdateProductRequestDto>(productDetails);
        return View(updateProductDto);
    }

    [AuthorizeApprovedCompany]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateProductRequestDto updateProductDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(updateProductDto);
        }

        var result = await _serviceManager.ProductService.UpdateAsync(id, updateProductDto, token);
        return RedirectToAction("Index");
    }
}
