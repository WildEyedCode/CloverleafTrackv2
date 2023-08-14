using CloverleafTrack.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/season")]
public class SeasonController : Controller
{
    private readonly ISeasonService seasonService;

    public SeasonController(ISeasonService seasonService)
    {
        this.seasonService = seasonService;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
}