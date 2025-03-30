using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class CategoryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
