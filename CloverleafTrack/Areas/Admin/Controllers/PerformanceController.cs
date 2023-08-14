using CloverleafTrack.Areas.Admin.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/performance")]
public class PerformanceController : Controller
{
    private readonly IPerformanceService performanceService;

    public PerformanceController(IPerformanceService performanceService)
    {
        this.performanceService = performanceService;
    }
    
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }
}