using CloverleafTrack.Services;

using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Controllers;

[Route("leaderboard")]
public class LeaderboardController : Controller
{
    private readonly ILeaderboardService leaderboardService;
    private readonly IEventService eventService;

    public LeaderboardController(ILeaderboardService leaderboardService, IEventService eventService)
    {
        this.leaderboardService = leaderboardService;
        this.eventService = eventService;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        var vm = leaderboardService.GetFullLeaderboard();
        return View(vm);
    }

    [Route("{eventName}")]
    public IActionResult EventLeaderboard(string eventName)
    {
        var @event = eventService.GetEventByName(eventName);

        if (@event.FieldEvent != null)
        {
            var vm = leaderboardService.GetFieldEventLeaderboard(@event.FieldEvent);
            return View("FieldEventLeaderboard", vm);
        }

        if (@event.FieldRelayEvent != null)
        {
            var vm = leaderboardService.GetFieldRelayEventLeaderboard(@event.FieldRelayEvent);
            return View("FieldRelayEventLeaderboard", vm);
        }

        if (@event.RunningEvent != null)
        {
            var vm = leaderboardService.GetRunningEventLeaderboard(@event.RunningEvent);
            return View("RunningEventLeaderboard", vm);
        }

        if (@event.RunningRelayEvent != null)
        {
            var vm = leaderboardService.GetRunningRelayEventLeaderboard(@event.RunningRelayEvent);
            return View("RunningRelayEventLeaderboard", vm);
        }

        return NotFound(eventName);
    }

    [Route("{eventName}/prs")]
    public IActionResult EventPrLeaderboard(string eventName)
    {
        var @event = eventService.GetEventByName(eventName);

        if (@event.FieldEvent != null)
        {
            var vm = leaderboardService.GetFieldEventLeaderboard(@event.FieldEvent, true);
            return View("FieldEventLeaderboard", vm);
        }

        if (@event.FieldRelayEvent != null)
        {
            var vm = leaderboardService.GetFieldRelayEventLeaderboard(@event.FieldRelayEvent, true);
            return View("FieldRelayEventLeaderboard", vm);
        }

        if (@event.RunningEvent != null)
        {
            var vm = leaderboardService.GetRunningEventLeaderboard(@event.RunningEvent, true);
            return View("RunningEventLeaderboard", vm);
        }

        if (@event.RunningRelayEvent != null)
        {
            var vm = leaderboardService.GetRunningRelayEventLeaderboard(@event.RunningRelayEvent, true);
            return View("RunningRelayEventLeaderboard", vm);
        }

        return NotFound(eventName);
    }
    
}
