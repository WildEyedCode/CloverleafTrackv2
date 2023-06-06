using System.Data;
using System.Diagnostics;

using CloverleafTrack.Managers;
using CloverleafTrack.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

public class HomeController : Controller
{
    private readonly IAthleteManager athleteManager;
    private readonly IEventManager eventManager;
    private readonly IMeetManager meetManager;
    private readonly IPerformanceManager performanceManager;
    private readonly ISeasonManager seasonManager; 
    
    public HomeController(IAthleteManager athleteManager, IEventManager eventManager, IMeetManager meetManager, IPerformanceManager performanceManager, ISeasonManager seasonManager)
    {
        this.athleteManager = athleteManager;
        this.eventManager = eventManager;
        this.meetManager = meetManager;
        this.performanceManager = performanceManager;
        this.seasonManager = seasonManager;
    }

    public async Task<IActionResult> Index(CancellationToken token)
    {
        if (meetManager.Count == 0)
        {
            await ReloadAsync(token);
        }
        
        var vm = new HomeViewModel(meetManager.Count, performanceManager.Count, athleteManager.Count, seasonManager.Count);
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task ReloadAsync(CancellationToken token = default)
    {
        await athleteManager.ReloadAsync(token);
        await eventManager.ReloadAsync(token);
        await meetManager.ReloadAsync(token);
        await performanceManager.ReloadAsync(token);
        await seasonManager.ReloadAsync(token);
    }
}
