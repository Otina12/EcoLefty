using EcoLefty.Application.Accounts.DTOs;
using EcoLefty.Application.ApplicationUsers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(CreateApplicationUserRequestDto registerDto)
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginAccountRequestDto loginDto)
    {
        return View();
    }
}
