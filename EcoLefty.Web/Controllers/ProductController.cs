using EcoLefty.Application.Contracts;
using EcoLefty.Application.Products.DTOs;
using EcoLefty.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;
public class ProductController : Controller
{
    private readonly IServiceManager _serviceManager;

    public ProductController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
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
        var offerDetailsDto = await _serviceManager.ProductService.GetByIdAsync(id, token);
        return View(offerDetailsDto);
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
}
