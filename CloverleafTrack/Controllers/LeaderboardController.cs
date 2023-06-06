using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

public class LeaderboardController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}
