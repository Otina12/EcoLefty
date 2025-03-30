using Microsoft.AspNetCore.Mvc;

namespace EcoLefty.Web.Areas.Company.Controllers;

[Area("Company")]
public class OfferController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
