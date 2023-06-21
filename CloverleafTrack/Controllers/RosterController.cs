using CloverleafTrack.Services;

using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

[Route("roster")]
public class RosterController : Controller
{
    private readonly IAthleteService athleteService;

    public RosterController(IAthleteService athleteService)
    {
        this.athleteService = athleteService;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        var vm = athleteService.GetRoster();
        return View(vm);
    }

    [Route("{name}")]
    public IActionResult Athlete(string name)
    {
        return View();
    }
}
