using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

public class RosterController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}
