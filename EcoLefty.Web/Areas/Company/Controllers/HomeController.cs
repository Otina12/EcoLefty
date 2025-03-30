using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Areas.Company.Controllers;

[Area("Company")]
public class HomeController : Controller
{
    public HomeController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
