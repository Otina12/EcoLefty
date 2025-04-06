using EcoLefty.Application;
using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers.DTOs;
using EcoLefty.Application.Authentication;
using EcoLefty.Application.Companies.DTOs;
using EcoLefty.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class AccountController : Controller
{
    private readonly IServiceManager _serviceManager;
    private readonly IAuthenticationService _authenticationService;

    public AccountController(IServiceManager serviceManager, IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
        _serviceManager = serviceManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RegisterUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(CreateApplicationUserRequestDto createUserDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(createUserDto);
        }

        await _serviceManager.ApplicationUserService.CreateAsync(createUserDto, token);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult RegisterCompany()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegisterCompany(CreateCompanyRequestDto createCompanyDto, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return View(createCompanyDto);
        }

        await _serviceManager.CompanyService.CreateAsync(createCompanyDto, token);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginAccountRequestDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        await _authenticationService.LoginAccountAsync(loginDto);
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _authenticationService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Profile(string id, CancellationToken cancellationToken)
    {
        var account = await _serviceManager.AccountService.GetAccountByIdAsync(id, cancellationToken);

        if (account.AccountType == AccountRole.Company)
        {
            var company = await _serviceManager.CompanyService.GetByIdAsync(id, cancellationToken);
            return RedirectToAction("Profile", "Company", new { id = company.Id });
        }
        else if (account.AccountType == AccountRole.User)
        {
            var user = await _serviceManager.ApplicationUserService.GetByIdAsync(id, cancellationToken);
            return RedirectToAction("Profile", "User", new { id = user.Id });
        }

        return RedirectToAction("Index", "Home");
    }
}
