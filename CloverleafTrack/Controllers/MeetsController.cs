using CloverleafTrack.Services;

using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

[Route("meets")]
public class MeetsController : Controller
{
    private readonly IMeetService meetService;
    private readonly ISeasonService seasonService;

    public MeetsController(IMeetService meetService, ISeasonService seasonService)
    {
        this.meetService = meetService;
        this.seasonService = seasonService;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        var vm = seasonService.GetSeasonsWithMeets();
        return View(vm);
    }

    [Route("{meetName}")]
    public IActionResult Details(string meetName)
    {
        var vm = meetService.GetMeetDetailsByName(meetName);
        if (vm != null)
        {
            return View(vm);
        }

        return NotFound(meetName);
    }
}
