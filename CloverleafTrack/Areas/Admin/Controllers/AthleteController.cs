using CloverleafTrack.Areas.Admin.Services;
using CloverleafTrack.Areas.Admin.ViewModels;
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
    public IActionResult Index()
    {
        var vm = new AllAthletesViewModel(athleteService.ReadAll());
        return View(vm);
    }

    // [Route("{sortOrder}")]
    // [HttpGet]
    // public IActionResult Index(string sortOrder)
    // {
    // }

    // [Route("{sortOrder}/{searchString}")]
    // [HttpGet]
    // public IActionResult Index(string sortOrder, string searchString)
    // {
    // }

    // [Route("{sortOrder}/{currentFilter}/{searchString}/{pageNumber}")]
    // [HttpGet]
    // public IActionResult Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    // {
    // }

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
                await athleteService.Create(athlete, token);
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
    public async Task<IActionResult> EditPost(Guid? id)
    {
        if (!id.HasValue)
        {
            return NotFound();
        }

        var athleteToUpdate = athleteService.ReadById(id.Value);
        if (athleteToUpdate == null)
        {
            return NotFound(id);
        }

        if (await TryUpdateModelAsync(
                athleteToUpdate,
                "",
                x => x.FirstName, x => x.LastName, x => x.Gender, x => x.GraduationYear))
        {
            try
            {
                await athleteService.Update(athleteToUpdate);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.ToString());
            }
        }

        return View();
    }

    [Route("delete/{id}/{saveChangesError}")]
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
    public async Task<IActionResult> DeleteConfirmed(Guid? id)
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
            await athleteService.Delete(athlete);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            return RedirectToAction(nameof(Delete), new { id = id.Value, saveChangesError = true });
        }
    }
}