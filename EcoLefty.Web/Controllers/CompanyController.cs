using EcoLefty.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class CompanyController : Controller
{
    private readonly IServiceManager _serviceManager;

    public CompanyController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public async Task<IActionResult> Profile(string id, CancellationToken token)
    {
        var companyDetailsDto = await _serviceManager.CompanyService.GetByIdAsync(id, token);
        return View(companyDetailsDto);
    }
}
