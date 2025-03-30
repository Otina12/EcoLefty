using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Controllers;

public class OfferController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
