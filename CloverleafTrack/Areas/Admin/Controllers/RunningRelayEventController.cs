using CloverleafTrack.Areas.Admin.Services;
using CloverleafTrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/runningrelayevent")]
public class RunningRelayEventController : Controller
{
    private readonly IRunningRelayEventService eventService;

    public RunningRelayEventController(IRunningRelayEventService eventService)
    {
        this.eventService = eventService;
    }

    [Route("")]
    [HttpGet]
    public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParameter"] = string.IsNullOrEmpty(sortOrder) ? "name_descending" : string.Empty;
        ViewData["GenderSortParameter"] = sortOrder == "gender_ascending" ? "gender_descending" : "gender_ascending";
        ViewData["EnvironmentSortParameter"] = sortOrder == "environment_ascending" ? "environment_descending" : "environment_ascending";
        ViewData["SortOrderSortParameter"] = sortOrder == "sortOrder_ascending" ? "sortOrder_descending" : "sortOrder_ascending";

        if (!string.IsNullOrEmpty(searchString))
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var events = eventService.ReadAll();
        if (!string.IsNullOrEmpty(searchString))
        {
            events = events.Where(x => x.Name.ToLower().Contains(searchString.ToLower())).ToList();
        }

        switch (sortOrder)
        {
            case "name_descending":
                events = events.OrderByDescending(x => x.Name).ToList();
                break;
            case "gender_ascending":
                events = events.OrderBy(x => x.Gender).ToList();
                break;
            case "gender_descending":
                events = events.OrderByDescending(x => x.Gender).ToList();
                break;
            case "environment_ascending":
                events = events.OrderBy(x => x.Environment).ToList();
                break;
            case "environment_descending":
                events = events.OrderByDescending(x => x.Environment).ToList();
                break;
            case "sortOrder_ascending":
                events = events.OrderBy(x => x.SortOrder).ToList();
                break;
            case "sortOrder_descending":
                events = events.OrderByDescending(x => x.SortOrder).ToList();
                break;
            default:
                events = events.OrderBy(x => x.Name).ToList();
                break;
        }

        int pageSize = events.Count;
        return View(PaginatedList<RunningRelayEvent>.Create(events, pageNumber ?? 1, pageSize));
    }
    
    [Route("{id}")]
    [HttpGet]
    public IActionResult Details(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var @event = eventService.ReadById(id.Value);
        if (@event == null)
        {
            return NotFound(id.Value);
        }

        return View(@event);
    }

    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Route("create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Gender,SortOrder,Environment")] RunningRelayEvent @event, CancellationToken token)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await eventService.CreateAsync(@event, token);
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.ToString());
        }

        return View(@event);
    }

    [Route("edit/{id}")]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var @event = eventService.ReadById(id.Value);
        if (@event == null)
        {
            return NotFound(id.Value);
        }

        return View(@event);
    }

    [Route("edit/{id}")]
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(Guid? id, [Bind("Id,Name,Gender,SortOrder,Environment")] RunningRelayEvent @event, CancellationToken token)
    {
        if (!id.HasValue || id.Value != @event.Id)
        {
            return NotFound();
        }

        var eventToUpdate = eventService.ReadById(id.Value);
        if (eventToUpdate == null)
        {
            return NotFound(id);
        }

        if (ModelState.IsValid)
        {
            try
            {
                eventToUpdate.Name = @event.Name;
                eventToUpdate.Gender = @event.Gender;
                eventToUpdate.SortOrder = @event.SortOrder;
                eventToUpdate.Environment = @event.Environment;
                
                await eventService.UpdateAsync(eventToUpdate, token);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
        }

        return View(eventToUpdate);
    }

    [Route("delete/{id}/{saveChangesError?}")]
    [HttpGet]
    public IActionResult Delete(Guid? id, bool? saveChangesError = false)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var @event = eventService.ReadById(id.Value);
        if (@event == null)
        {
            return NotFound(id.Value);
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ViewData["ErrorMessage"] = "Delete failed. Try again.";
        }

        return View(@event);
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

        var @event = eventService.ReadById(id.Value);
        if (@event == null)
        {
            return NotFound(id);
        }

        try
        {
            await eventService.DeleteAsync(@event, token);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Delete), new { id = id.Value, saveChangesError = true });
        }
    }
}