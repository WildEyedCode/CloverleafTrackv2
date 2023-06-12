using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

[Route("roster")]
public class RosterController : Controller
{
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("{name}")]
    public IActionResult Athlete(string name)
    {
        return View();
    }
}
