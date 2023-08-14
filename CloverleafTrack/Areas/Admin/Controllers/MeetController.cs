using CloverleafTrack.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/meet")]
public class MeetController : Controller
{
    private readonly IMeetService meetService;

    public MeetController(IMeetService meetService)
    {
        this.meetService = meetService;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
}