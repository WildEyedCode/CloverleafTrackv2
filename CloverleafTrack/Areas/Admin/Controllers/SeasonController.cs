using CloverleafTrack.Areas.Admin.Services;
using CloverleafTrack.Models;
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
    [HttpGet]
    public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParameter"] = string.IsNullOrEmpty(sortOrder) ? "name_descending" : string.Empty;

        if (!string.IsNullOrEmpty(searchString))
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var seasons = seasonService.ReadAll();
        if (!string.IsNullOrEmpty(searchString))
        {
            seasons = seasons.Where(x =>
                x.Name.ToLower().Contains(searchString.ToLower())).ToList();
        }

        switch (sortOrder)
        {
            case "name_ascending":
                seasons = seasons.OrderBy(x => x.Name).ToList();
                break;
            case "name_descending":
                seasons = seasons.OrderByDescending(x => x.Name).ToList();
                break;
            default:
                seasons = seasons.OrderBy(x => x.Name).ToList();
                break;
        }

        int pageSize = seasons.Count;
        return View(PaginatedList<Season>.Create(seasons, pageNumber ?? 1, pageSize));
    }

    [Route("{id}")]
    [HttpGet]
    public IActionResult Details(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var season = seasonService.ReadById(id.Value);
        if (season == null)
        {
            return NotFound(id.Value);
        }

        return View(season);
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
    public async Task<IActionResult> Create([Bind("Name")] Season season, CancellationToken token)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await seasonService.CreateAsync(season, token);
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.ToString());
        }

        return View(season);
    }
    
    [Route("edit/{id}")]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var season = seasonService.ReadById(id.Value);
        if (season == null)
        {
            return NotFound(id.Value);
        }

        return View(season);
    }
    
    [Route("edit/{id}")]
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(Guid? id, [Bind("Id,Name")] Season season, CancellationToken token)
    {
        if (!id.HasValue || id.Value != season.Id)
        {
            return NotFound();
        }

        var seasonToUpdate = seasonService.ReadById(id.Value);
        if (seasonToUpdate == null)
        {
            return NotFound(id);
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                seasonToUpdate.Name = season.Name;
                
                await seasonService.UpdateAsync(seasonToUpdate, token);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
        }

        return View(seasonToUpdate);
    }
    
    [Route("delete/{id}/{saveChangesError?}")]
    [HttpGet]
    public IActionResult Delete(Guid? id, bool? saveChangesError = false)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var season = seasonService.ReadById(id.Value);
        if (season == null)
        {
            return NotFound(id.Value);
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ViewData["ErrorMessage"] = "Delete failed. Try again.";
        }

        return View(season);
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

        var season = seasonService.ReadById(id.Value);
        if (season == null)
        {
            return NotFound(id);
        }

        try
        {
            await seasonService.DeleteAsync(season, token);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Delete), new { id = id.Value, saveChangesError = true });
        }
    }
}