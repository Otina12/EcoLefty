using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
