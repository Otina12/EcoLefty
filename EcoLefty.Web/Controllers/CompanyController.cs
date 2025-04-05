using AutoMapper;
using EcoLefty.Application;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class CompanyController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IMapper _mapper;

    public CompanyController(IServiceManager serviceManager, IMapper mapper)
    {
        _serviceManager = serviceManager;
        _mapper = mapper;
    }

    public async Task<IActionResult> Profile(string id, CancellationToken token)
    {
        var companyDetailsDto = await _serviceManager.CompanyService.GetByIdAsync(id, token);
        return View(companyDetailsDto);
    }

    [AllowOnlyOwnerAccount]
    [HttpGet]
    public async Task<IActionResult> Edit(string id, CancellationToken token)
    {
        var companyDetailsDto = await _serviceManager.CompanyService.GetByIdAsync(id, token);
        var updateCompanyDto = _mapper.Map<UpdateCompanyRequestDto>(companyDetailsDto);
        return View(updateCompanyDto);
    }

    [AllowOnlyOwnerAccount]
    [HttpPost]
    public async Task<IActionResult> Edit(string id, UpdateCompanyRequestDto updateCompanyDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(updateCompanyDto);
        }

        var result = await _serviceManager.CompanyService.UpdateAsync(id, updateCompanyDto, token);
        return RedirectToAction("Profile", "Company", new { id = id });
    }
}
