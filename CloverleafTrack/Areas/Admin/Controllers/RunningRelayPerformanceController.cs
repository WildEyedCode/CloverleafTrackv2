using CloverleafTrack.Areas.Admin.Services;
using CloverleafTrack.Areas.Admin.ViewModels;
using CloverleafTrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/RunningRelayPerformance")]
public class RunningRelayPerformanceController : Controller
{
    private readonly IRunningRelayPerformanceService performanceService;
    private readonly IRunningRelayEventService eventService;
    private readonly IMeetService meetService;
    private readonly IAthleteService athleteService;

    public RunningRelayPerformanceController(IRunningRelayPerformanceService performanceService, IRunningRelayEventService eventService, IMeetService meetService, IAthleteService athleteService)
    {
        this.performanceService = performanceService;
        this.eventService = eventService;
        this.meetService = meetService;
        this.athleteService = athleteService;
    }

    [Route("")]
    [HttpGet]
    public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["EventSortParameter"] = string.IsNullOrEmpty(sortOrder) ? SortParameters.EventDescending : string.Empty;
        ViewData["MeetSortParameter"] = sortOrder == SortParameters.MeetAscending ? SortParameters.MeetDescending : SortParameters.MeetAscending;
        ViewData["PerformanceSortParameter"] = sortOrder == SortParameters.PerformanceAscending ? SortParameters.PerformanceDescending : SortParameters.PerformanceAscending;

        if (!string.IsNullOrEmpty(searchString))
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var performances = performanceService.ReadAll();
        if (!string.IsNullOrEmpty(searchString))
        {
            performances = performances.Where(x => x.Athletes.Any(y => y.Name.ToLower().Contains(searchString.ToLower()))).ToList();
        }

        switch (sortOrder)
        {
            case SortParameters.EventDescending:
                performances = performances.OrderByDescending(x => x.Event.Name).ToList();
                break;
            case SortParameters.MeetAscending:
                performances = performances.OrderBy(x => x.Meet.Name).ToList();
                break;
            case SortParameters.MeetDescending:
                performances = performances.OrderByDescending(x => x.Meet.Name).ToList();
                break;
            case SortParameters.PerformanceAscending:
                performances = performances.OrderBy(x => x.Time).ToList();
                break;
            case SortParameters.PerformanceDescending:
                performances = performances.OrderByDescending(x => x.Time).ToList();
                break;
            default:
                performances = performances.OrderBy(x => x.Event.Name).ToList();
                break;
        }

        int pageSize = performances.Count;
        return View(PaginatedList<RunningRelayPerformance>.Create(performances, pageNumber ?? 1, pageSize));
    }
    
    [Route("{id}")]
    [HttpGet]
    public IActionResult Details(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var performance = performanceService.ReadById(id.Value);
        if (performance == null)
        {
            return NotFound(id.Value);
        }

        return View(performance);
    }
    
    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
        var vm = new FullRunningRelayPerformanceViewModel(new RunningRelayPerformance(), new Guid(), new Guid(), new Guid(), new Guid(), eventService.ReadAll(), meetService.ReadActive(), athleteService.ReadAll());
        return View(vm);
    }
    
    [Route("create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventId,MeetId,Minutes,Seconds")] RunningRelayPerformance performance, Guid athleteId1, Guid athleteId2, Guid athleteId3, Guid athleteId4, CancellationToken token)
    {
        try
        {
            if (ModelState.IsValid)
            {
                performance.AthleteIds = new List<Guid> { athleteId1, athleteId2, athleteId3, athleteId4 };
                await performanceService.CreateAsync(performance, token);
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.ToString());
        }

        var vm = new FullRunningRelayPerformanceViewModel(performance, athleteId1, athleteId2, athleteId3, athleteId4, eventService.ReadAll(), meetService.ReadActive(), athleteService.ReadAll());
        return View(vm);
    }
    
    [Route("edit/{id}")]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var performance = performanceService.ReadById(id.Value);
        if (performance == null)
        {
            return NotFound(id.Value);
        }
        
        var athletes = performance.Athletes;
        var athleteId1 = new Guid();
        var athleteId2 = new Guid();
        var athleteId3 = new Guid();
        var athleteId4 = new Guid();

        if (athletes.Count == 1)
        {
            athleteId1 = athletes[0].Id;
        }
        else if (athletes.Count == 2)
        {
            athleteId1 = athletes[0].Id;
            athleteId2 = athletes[1].Id;
        }
        else if (athletes.Count == 3)
        {
            athleteId1 = athletes[0].Id;
            athleteId2 = athletes[1].Id;
            athleteId3 = athletes[2].Id;
        }
        else if (athletes.Count == 4)
        {
            athleteId1 = athletes[0].Id;
            athleteId2 = athletes[1].Id;
            athleteId3 = athletes[2].Id;
            athleteId4 = athletes[3].Id;
        }

        var vm = new FullRunningRelayPerformanceViewModel(performance, athleteId1, athleteId2, athleteId3, athleteId4, eventService.ReadAll(), meetService.ReadAll(), athleteService.ReadAll());
        return View(vm);
    }
    
    [Route("edit/{id}")]
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(Guid? id, [Bind("Id,EventId,MeetId,Minutes,Seconds")] RunningRelayPerformance performance, Guid athleteId1, Guid athleteId2, Guid athleteId3, Guid athleteId4, CancellationToken token)
    {
        if (!id.HasValue || id.Value != performance.Id)
        {
            return NotFound();
        }

        var performanceToUpdate = performanceService.ReadById(id.Value);
        if (performanceToUpdate == null)
        {
            return NotFound(id);
        }

        if (ModelState.IsValid)
        {
            try
            {
                performanceToUpdate.AthleteIds = new List<Guid> { athleteId1, athleteId2, athleteId3, athleteId4 };
                performanceToUpdate.EventId = performance.EventId;
                performanceToUpdate.MeetId = performance.MeetId;
                performanceToUpdate.Minutes = performance.Minutes;
                performanceToUpdate.Seconds = performance.Seconds;
                
                await performanceService.UpdateAsync(performanceToUpdate, token);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
        }

        var vm = new FullRunningRelayPerformanceViewModel(performanceToUpdate, athleteId1, athleteId2, athleteId3, athleteId4, eventService.ReadAll(), meetService.ReadAll(), athleteService.ReadAll());
        return View(vm);
    }
    
    [Route("delete/{id}/{saveChangesError?}")]
    [HttpGet]
    public IActionResult Delete(Guid? id, bool? saveChangesError = false)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var performance = performanceService.ReadById(id.Value);
        if (performance == null)
        {
            return NotFound(id.Value);
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ViewData["ErrorMessage"] = "Delete failed. Try again.";
        }

        return View(performance);
    }
    
    [Route("delete/{id}")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid? id, CancellationToken token)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var performance = performanceService.ReadById(id.Value);
        if (performance == null)
        {
            return NotFound(id);
        }

        try
        {
            await performanceService.DeleteAsync(performance, token);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Delete), new { id = id.Value, saveChangesError = true });
        }
    }
}