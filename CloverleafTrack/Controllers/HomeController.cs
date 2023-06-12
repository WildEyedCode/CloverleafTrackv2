using System.Data;
using System.Diagnostics;

using CloverleafTrack.Services;
using CloverleafTrack.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

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

    public async Task<IActionResult> Index(CancellationToken token)
    {
        if (meetService.Count == 0)
        {
            await ReloadAsync(token);
        }
        
        var vm = new HomeViewModel(meetService.Count, performanceService.Count, athleteService.Count, seasonService.Count);
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
