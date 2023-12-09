using CloverleafTrack.Areas.Admin.Services;
using CloverleafTrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloverleafTrack.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/athlete")]
public class AthleteController : Controller
{
    private readonly IAthleteService athleteService;

    public AthleteController(IAthleteService athleteService)
    {
        this.athleteService = athleteService;
    }

    [Route("")]
    [HttpGet]
    public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["FirstNameSortParameter"] = string.IsNullOrEmpty(sortOrder) ? "firstName_descending" : string.Empty;
        ViewData["LastNameSortParameter"] = sortOrder == "lastName_ascending" ? "lastName_descending" : "lastName_ascending";
        ViewData["GenderSortParameter"] = sortOrder == "gender_ascending" ? "gender_descending" : "gender_ascending";
        ViewData["GraduationYearSortParameter"] = sortOrder == "graduationYear_ascending" ? "graduationYear_descending" : "graduationYear_ascending";
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

        var athletes = athleteService.ReadAll();
        if (!string.IsNullOrEmpty(searchString))
        {
            athletes = athletes.Where(x => x.FirstName.ToLower().Contains(searchString.ToLower()) || x.LastName.ToLower().Contains(searchString.ToLower())).ToList();
        }

        switch (sortOrder)
        {
            case "firstName_ascending":
                athletes = athletes.OrderBy(x => x.FirstName).ToList();
                break;
            case "firstName_descending":
                athletes = athletes.OrderByDescending(x => x.FirstName).ToList();
                break;
            case "lastName_ascending":
                athletes = athletes.OrderBy(x => x.LastName).ToList();
                break;
            case "lastName_descending":
                athletes = athletes.OrderByDescending(x => x.LastName).ToList();
                break;
            case "gender_ascending":
                athletes = athletes.OrderBy(x => x.Gender).ToList();
                break;
            case "gender_descending":
                athletes = athletes.OrderByDescending(x => x.Gender).ToList();
                break;
            case "graduationYear_ascending":
                athletes = athletes.OrderBy(x => x.GraduationYear).ToList();
                break;
            case "graduationYear_descending":
                athletes = athletes.OrderByDescending(x => x.GraduationYear).ToList();
                break;
            case "deleted_ascending":
                athletes = athletes.OrderBy(x => x.Deleted).ToList();
                break;
            case "deleted_descending":
                athletes = athletes.OrderByDescending(x => x.Deleted).ToList();
                break;
            default:
                athletes = athletes.OrderBy(x => x.FirstName).ToList();
                break;
        }

        int pageSize = athletes.Count;
        return View(PaginatedList<Athlete>.Create(athletes, pageNumber ?? 1, pageSize));
    }

    [Route("{id}")]
    [HttpGet]
    public IActionResult Details(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var athlete = athleteService.ReadById(id.Value);
        if (athlete == null)
        {
            return NotFound(id.Value);
        }

        return View(athlete);
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
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Gender,GraduationYear")] Athlete athlete, CancellationToken token)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await athleteService.CreateAsync(athlete, token);
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.ToString());
        }

        return View(athlete);
    }

    [Route("edit/{id}")]
    [HttpGet]
    public IActionResult Edit(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var athlete = athleteService.ReadById(id.Value);
        if (athlete == null)
        {
            return NotFound(id.Value);
        }

        return View(athlete);
    }

    [Route("edit/{id}")]
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPost(Guid? id, [Bind("Id,FirstName,LastName,Gender,GraduationYear")] Athlete athlete, CancellationToken token)
    {
        if (!id.HasValue || id.Value != athlete.Id)
        {
            return NotFound();
        }

        var athleteToUpdate = athleteService.ReadById(id.Value);
        if (athleteToUpdate == null)
        {
            return NotFound(id);
        }

        if (ModelState.IsValid)
        {
            try
            {
                athleteToUpdate.FirstName = athlete.FirstName;
                athleteToUpdate.LastName = athlete.LastName;
                athleteToUpdate.Gender = athlete.Gender;
                athleteToUpdate.GraduationYear = athlete.GraduationYear;
                
                await athleteService.UpdateAsync(athleteToUpdate, token);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
        }

        return View(athleteToUpdate);
    }

    [Route("delete/{id}/{saveChangesError?}")]
    [HttpGet]
    public IActionResult Delete(Guid? id, bool? saveChangesError = false)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var athlete = athleteService.ReadById(id.Value);
        if (athlete == null)
        {
            return NotFound(id.Value);
        }

        if (saveChangesError.GetValueOrDefault())
        {
            ViewData["ErrorMessage"] = "Delete failed. Try again.";
        }

        return View(athlete);
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

        var athlete = athleteService.ReadById(id.Value);
        if (athlete == null)
        {
            return NotFound(id);
        }

        try
        {
            await athleteService.DeleteAsync(athlete, token);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Delete), new { id = id.Value, saveChangesError = true });
        }
    }
}