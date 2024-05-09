using CloverleafTrack.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

using IAthleteServiceNonAdmin = CloverleafTrack.Services.IAthleteService;
using IEventServiceNonAdmin = CloverleafTrack.Services.IEventService;
using IMeetServiceNonAdmin = CloverleafTrack.Services.IMeetService;
using IPerformanceServiceNonAdmin = CloverleafTrack.Services.IPerformanceService;
using ISeasonServiceNonAdmin = CloverleafTrack.Services.ISeasonService;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin")]
public class HomeController : Controller
{
    private readonly IAthleteService athleteService;
    private readonly IFieldEventService fieldEventService;
    private readonly IFieldRelayEventService fieldRelayEventService;
    private readonly IRunningEventService runningEventService;
    private readonly IRunningRelayEventService runningRelayEventService;
    private readonly IMeetService meetService;
    private readonly ISeasonService seasonService;
    private readonly IFieldPerformanceService fieldPerformanceService;
    private readonly IFieldRelayPerformanceService fieldRelayPerformanceService;
    private readonly IRunningPerformanceService runningPerformanceService;
    private readonly IRunningRelayPerformanceService runningRelayPerformanceService;

    private readonly IAthleteServiceNonAdmin athleteServiceNonAdmin;
    private readonly IEventServiceNonAdmin eventServiceNonAdmin;
    private readonly IMeetServiceNonAdmin meetServiceNonAdmin;
    private readonly IPerformanceServiceNonAdmin performanceServiceNonAdmin;
    private readonly ISeasonServiceNonAdmin seasonServiceNonAdmin;
    
    public HomeController(
        IAthleteService athleteService,
        IFieldEventService fieldEventService,
        IFieldRelayEventService fieldRelayEventService,
        IRunningEventService runningEventService,
        IRunningRelayEventService runningRelayEventService,
        IMeetService meetService,
        ISeasonService seasonService,
        IFieldPerformanceService fieldPerformanceService,
        IFieldRelayPerformanceService fieldRelayPerformanceService,
        IRunningPerformanceService runningPerformanceService,
        IRunningRelayPerformanceService runningRelayPerformanceService,
        IAthleteServiceNonAdmin athleteServiceNonAdmin,
        IEventServiceNonAdmin eventServiceNonAdmin,
        IMeetServiceNonAdmin meetServiceNonAdmin,
        IPerformanceServiceNonAdmin performanceServiceNonAdmin,
        ISeasonServiceNonAdmin seasonServiceNonAdmin)
    {
        this.athleteService = athleteService;
        this.fieldEventService = fieldEventService;
        this.fieldRelayEventService = fieldRelayEventService;
        this.runningEventService = runningEventService;
        this.runningRelayEventService = runningRelayEventService;
        this.meetService = meetService;
        this.seasonService = seasonService;
        this.fieldPerformanceService = fieldPerformanceService;
        this.fieldRelayPerformanceService = fieldRelayPerformanceService;
        this.runningPerformanceService = runningPerformanceService;
        this.runningRelayPerformanceService = runningRelayPerformanceService;

        this.athleteServiceNonAdmin = athleteServiceNonAdmin;
        this.eventServiceNonAdmin = eventServiceNonAdmin;
        this.meetServiceNonAdmin = meetServiceNonAdmin;
        this.performanceServiceNonAdmin = performanceServiceNonAdmin;
        this.seasonServiceNonAdmin = seasonServiceNonAdmin;
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

    [Route("refresh")]
    public async Task<IActionResult> RefreshCache(CancellationToken token)
    {
        await ReloadAsync(token);
        await ReloadNonAdminAsync(token);

        return RedirectToAction(nameof(Index));
    }

    [Route("records")]
    public async Task<IActionResult> Records(CancellationToken token)
    {
        await ReloadAsync(token);
        await ReloadNonAdminAsync(token);
        
        await fieldPerformanceService.CalculateRecordsAsync(token);
        await fieldRelayPerformanceService.CalculateRecordsAsync(token);
        await runningPerformanceService.CalculateRecordsAsync(token);
        await runningRelayPerformanceService.CalculateRecordsAsync(token);
        
        return RedirectToAction(nameof(RefreshCache));
    }
    
    private async Task ReloadAsync(CancellationToken token = default)
    {
        await athleteService.ReloadAsync(token);
        await fieldEventService.ReloadAsync(token);
        await fieldRelayEventService.ReloadAsync(token);
        await runningEventService.ReloadAsync(token);
        await runningRelayEventService.ReloadAsync(token);
        await meetService.ReloadAsync(token);
        await seasonService.ReloadAsync(token);
        await fieldPerformanceService.ReloadAsync(token);
        await fieldRelayPerformanceService.ReloadAsync(token);
        await runningPerformanceService.ReloadAsync(token);
        await runningRelayPerformanceService.ReloadAsync(token);
    }

    private async Task ReloadNonAdminAsync(CancellationToken token = default)
    {
        await athleteServiceNonAdmin.ReloadAsync(token);
        await eventServiceNonAdmin.ReloadAsync(token);
        await meetServiceNonAdmin.ReloadAsync(token);
        await performanceServiceNonAdmin.ReloadAsync(token);
        await seasonServiceNonAdmin.ReloadAsync(token);
    }
}