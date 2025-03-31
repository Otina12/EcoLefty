using EcoLefty.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class UserController : Controller
{
    private readonly IServiceManager _serviceManager;

    public UserController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    public async Task<IActionResult> Profile(string id, CancellationToken token)
    {
        var userDetailsDto = await _serviceManager.ApplicationUserService.GetByIdAsync(id, token);
        return View(userDetailsDto);
    }


}
