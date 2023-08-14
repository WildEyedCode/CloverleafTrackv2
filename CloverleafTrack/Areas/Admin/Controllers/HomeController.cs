using CloverleafTrack.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin")]
public class HomeController : Controller
{
    private readonly IAthleteService athleteService;
    private readonly IEventService eventService;
    private readonly IMeetService meetService;
    private readonly IPerformanceService performanceService;
    private readonly ISeasonService seasonService;
    
    public HomeController(IAthleteService athleteService, IEventService eventService, IMeetService meetService, IPerformanceService performanceService, ISeasonService seasonService)
    {
        this.athleteService = athleteService;
        this.eventService = eventService;
        this.meetService = meetService;
        this.performanceService = performanceService;
        this.seasonService = seasonService;
    }
    
    [Route("")]
    public async Task<IActionResult> Index(CancellationToken token)
    {
        if (athleteService.Count == 0)
        {
            await ReloadAsync(token);
        }
        
        return View();
    }
    
    private async Task ReloadAsync(CancellationToken token = default)
    {
        await athleteService.ReloadAsync(token);
        await eventService.ReloadAsync(token);
        await meetService.ReloadAsync(token);
        await performanceService.ReloadAsync(token);
        await seasonService.ReloadAsync(token);
    }
}