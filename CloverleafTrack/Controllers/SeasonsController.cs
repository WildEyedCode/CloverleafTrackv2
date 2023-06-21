using CloverleafTrack.Services;
using CloverleafTrack.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

[Route("seasons")]
public class SeasonsController : Controller
{
    private readonly ISeasonService seasonService;

    public SeasonsController(ISeasonService seasonService)
    {
        this.seasonService = seasonService;
    }
    
    public IActionResult Index()
    {
        var vm = seasonService.GetAllSeasons();
        return View(vm);
    }

    [Route("{seasonName}")]
    public IActionResult Details(string seasonName)
    {
        var vm = seasonService.GetSeason(seasonName);
        if (vm == null)
        {
            return NotFound(seasonName);
        }
        return View(vm);
    }
}
