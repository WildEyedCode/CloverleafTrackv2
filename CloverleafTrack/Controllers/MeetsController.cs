using CloverleafTrack.Managers;

using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

[Route("meets")]
public class MeetsController : Controller
{
    private readonly IMeetManager meetManager;

    public MeetsController(IMeetManager meetManager)
    {
        this.meetManager = meetManager;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("{meetName}")]
    public async Task<IActionResult> Details(string meetName)
    {
        return View();
    }
}
