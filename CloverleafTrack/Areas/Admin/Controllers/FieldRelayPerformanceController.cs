using CloverleafTrack.Areas.Admin.Services;
using CloverleafTrack.Areas.Admin.ViewModels;
using CloverleafTrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/FieldRelayPerformance")]
public class FieldRelayPerformanceController : Controller
{
    private readonly IFieldRelayPerformanceService performanceService;
    private readonly IFieldRelayEventService eventService;
    private readonly IMeetService meetService;
    private readonly IAthleteService athleteService;

    public FieldRelayPerformanceController(IFieldRelayPerformanceService performanceService, IFieldRelayEventService eventService, IMeetService meetService, IAthleteService athleteService)
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
                performances = performances.OrderBy(x => x.Distance).ToList();
                break;
            case SortParameters.PerformanceDescending:
                performances = performances.OrderByDescending(x => x.Distance).ToList();
                break;
            default:
                performances = performances.OrderBy(x => x.Event.Name).ToList();
                break;
        }

        int pageSize = performances.Count;
        return View(PaginatedList<FieldRelayPerformance>.Create(performances, pageNumber ?? 1, pageSize));
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
        var vm = new FullFieldRelayPerformanceViewModel(new FieldRelayPerformance(), new Guid(), new Guid(), new Guid(), eventService.ReadAll(), meetService.ReadActive(), athleteService.ReadAll());
        return View(vm);
    }
    
    [Route("create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("EventId,MeetId,Feet,Inches")] FieldRelayPerformance performance, Guid athleteId1, Guid athleteId2, Guid athleteId3, CancellationToken token)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var @event = eventService.ReadById(performance.EventId);
                performance.AthleteIds = @event!.AthleteCount == 2
                    ? new List<Guid> { athleteId1, athleteId2 }
                    : new List<Guid> { athleteId1, athleteId2, athleteId3 };
                
                await performanceService.CreateAsync(performance, token);
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.ToString());
        }

        var vm = new FullFieldRelayPerformanceViewModel(performance, athleteId1, athleteId2, athleteId3, eventService.ReadAll(), meetService.ReadActive(), athleteService.ReadAll());
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
        var @event = eventService.ReadById(performance.EventId);
        Guid athleteId1, athleteId2, athleteId3;
        if (@event.AthleteCount == 2)
        {
            athleteId1 = athletes[0].Id;
            athleteId2 = athletes[1].Id;
            athleteId3 = athleteService.ReadAll().First().Id;
        }
        else
        {
            athleteId1 = athletes[0].Id;
            athleteId2 = athletes[1].Id;
            athleteId3 = athletes[2].Id;
        }

        var vm = new FullFieldRelayPerformanceViewModel(performance, athleteId1, athleteId2, athleteId3, eventService.ReadAll(), meetService.ReadAll(), athleteService.ReadAll());
        return View(vm);
    }
    
    [Route("edit/{id}")]
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(Guid? id, [Bind("Id,EventId,MeetId,Feet,Inches")] FieldRelayPerformance performance, Guid athleteId1, Guid athleteId2, Guid athleteId3, CancellationToken token)
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
                var @event = eventService.ReadById(performance.EventId);
                performanceToUpdate.AthleteIds = @event!.AthleteCount == 2
                    ? new List<Guid> { athleteId1, athleteId2 }
                    : new List<Guid> { athleteId1, athleteId2, athleteId3 };
                
                performanceToUpdate.EventId = performance.EventId;
                performanceToUpdate.MeetId = performance.MeetId;
                performanceToUpdate.Feet = performance.Feet;
                performanceToUpdate.Inches = performance.Inches;
                
                await performanceService.UpdateAsync(performanceToUpdate, token);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
        }

        var vm = new FullFieldRelayPerformanceViewModel(performanceToUpdate, athleteId1, athleteId2, athleteId3, eventService.ReadAll(), meetService.ReadAll(), athleteService.ReadAll());
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