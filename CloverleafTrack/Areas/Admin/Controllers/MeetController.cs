using CloverleafTrack.Areas.Admin.Services;
using CloverleafTrack.Areas.Admin.ViewModels;
using CloverleafTrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/meet")]
public class MeetController : Controller
{
    private readonly IMeetService meetService;
    private readonly ISeasonService seasonService;

    public MeetController(IMeetService meetService, ISeasonService seasonService)
    {
        this.meetService = meetService;
        this.seasonService = seasonService;
    }
    
    [Route("")]
    [HttpGet]
    public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParameter"] = string.IsNullOrEmpty(sortOrder) ? "name_descending" : string.Empty;
        ViewData["LocationSortParameter"] = sortOrder == "location_ascending" ? "location_descending" : "location_ascending";
        ViewData["DateSortParameter"] = sortOrder == "date_ascending" ? "date_descending" : "date_ascending";
        ViewData["EnvironmentSortParameter"] = sortOrder == "environment_ascending" ? "environment_descending" : "environment_ascending";
        ViewData["DeletedSortParameter"] = sortOrder == "deleted_ascending" ? "deleted_descending" : "deleted_ascending";

        if (!string.IsNullOrEmpty(searchString))
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var meets = meetService.ReadAll();
        if (!string.IsNullOrEmpty(searchString))
        {
            meets = meets.Where(x =>
                x.Name.ToLower().Contains(searchString.ToLower()) ||
                x.Location.ToLower().Contains(searchString.ToLower())).ToList();
        }

        switch (sortOrder)
        {
            case "name_ascending":
                meets = meets.OrderBy(x => x.Name).ToList();
                break;
            case "name_descending":
                meets = meets.OrderByDescending(x => x.Name).ToList();
                break;
            case "location_ascending":
                meets = meets.OrderBy(x => x.Location).ToList();
                break;
            case "location_descending":
                meets = meets.OrderByDescending(x => x.Location).ToList();
                break;
            case "date_ascending":
                meets = meets.OrderBy(x => x.Date).ToList();
                break;
            case "date_descending":
                meets = meets.OrderByDescending(x => x.Date).ToList();
                break;
            case "environment_ascending":
                meets = meets.OrderBy(x => x.Environment).ToList();
                break;
            case "environment_descending":
                meets = meets.OrderByDescending(x => x.Environment).ToList();
                break;
            case "deleted_ascending":
                meets = meets.OrderBy(x => x.Deleted).ToList();
                break;
            case "deleted_descending":
                meets = meets.OrderByDescending(x => x.Deleted).ToList();
                break;
            default:
                meets = meets.OrderBy(x => x.Name).ToList();
                break;
        }

        int pageSize = meets.Count;
        return View(PaginatedList<Meet>.Create(meets, pageNumber ?? 1, pageSize));
    }

    [Route("{id}")]
    [HttpGet]
    public IActionResult Details(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var meet = meetService.ReadById(id.Value);
        if (meet == null)
        {
            return NotFound(id.Value);
        }

        return View(meet);
    }

    [Route("create")]
    [HttpGet]
    public IActionResult Create()
    {
        var vm = new MeetAndSeasonsViewModel(new Meet(), seasonService.ReadAll());
        return View(vm);
    }

    [Route("create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name","Date","Location","Environment","SeasonId")] Meet meet, CancellationToken token)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await meetService.CreateAsync(meet, token);
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.ToString());
        }

        var vm = new MeetAndSeasonsViewModel(meet, seasonService.ReadAll());
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

        var meet = meetService.ReadById(id.Value);
        if (meet == null)
        {
            return NotFound(id.Value);
        }

        var vm = new MeetAndSeasonsViewModel(meet, seasonService.ReadAll());
        return View(vm);
    }
    
    [Route("edit/{id}")]
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(Guid? id, [Bind("Id,Name,Date,Location,Environment,SeasonId")] Meet meet, CancellationToken token)
    {
        if (!id.HasValue || id.Value != meet.Id)
        {
            return NotFound();
        }

        var meetToUpdate = meetService.ReadById(id.Value);
        if (meetToUpdate == null)
        {
            return NotFound(id);
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                meetToUpdate.Name = meet.Name;
                meetToUpdate.Date = meet.Date;
                meetToUpdate.Location = meet.Location;
                meetToUpdate.Environment = meet.Environment;
                meetToUpdate.SeasonId = meet.SeasonId;
                
                await meetService.UpdateAsync(meetToUpdate, token);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
        }

        var vm = new MeetAndSeasonsViewModel(meetToUpdate, seasonService.ReadAll());
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

        var meet = meetService.ReadById(id.Value);
        if (meet == null)
        {
            return NotFound(id.Value);
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ViewData["ErrorMessage"] = "Delete failed. Try again.";
        }

        return View(meet);
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

        var meet = meetService.ReadById(id.Value);
        if (meet == null)
        {
            return NotFound(id);
        }

        try
        {
            await meetService.DeleteAsync(meet, token);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Delete), new { id = id.Value, saveChangesError = true });
        }
    }
}