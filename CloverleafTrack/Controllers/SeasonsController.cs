using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

public class SeasonsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}
