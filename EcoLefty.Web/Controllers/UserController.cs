using AutoMapper;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Contracts;
using EcoLefty.Web.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class UserController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IMapper _mapper;

    public UserController(IServiceManager serviceManager, IMapper mapper)
    {
        _serviceManager = serviceManager;
        _mapper = mapper;
    }

    public async Task<IActionResult> Profile(string id, CancellationToken token)
    {
        var userDetailsDto = await _serviceManager.ApplicationUserService.GetByIdAsync(id, token);
        return View(userDetailsDto);
    }

    [AllowOnlyOwnerAccount]
    [HttpGet]
    public async Task<IActionResult> Edit(string id, CancellationToken token)
    {
        var userDetailsDto = await _serviceManager.ApplicationUserService.GetByIdAsync(id, token);
        var updateuserDto = _mapper.Map<UpdateApplicationUserRequestDto>(userDetailsDto);
        return View(updateuserDto);
    }

    [AllowOnlyOwnerAccount]
    [HttpPost]
    public async Task<IActionResult> Edit(string id, UpdateApplicationUserRequestDto updateUserDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(updateUserDto);
        }

        var result = await _serviceManager.ApplicationUserService.UpdateAsync(id, updateUserDto, token);
        return RedirectToAction("Profile", "User", new { id = id });
    }
}
