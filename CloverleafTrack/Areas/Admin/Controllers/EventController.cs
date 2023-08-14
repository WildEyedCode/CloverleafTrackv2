using CloverleafTrack.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/event")]
public class EventController : Controller
{
    private readonly IEventService eventService;

    public EventController(IEventService eventService)
    {
        this.eventService = eventService;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
}